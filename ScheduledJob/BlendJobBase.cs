using EPiServer;
using EPiServer.Core;
using EPiServer.Data;
using EPiServer.DataAbstraction;
using EPiServer.Framework.Blobs;
using EPiServer.Scheduler;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Blend.Optimizely.ScheduledJobs
{
    public abstract class BlendJobBase : ScheduledJobBase
    {
        protected readonly IContentLoader contentLoader;

        protected readonly IContentTypeRepository contentTypeRepository;

        protected readonly IContentRepository contentRepository;

        protected readonly SiteDefinition siteDefinition;

        protected readonly UrlResolver urlResolver;

        protected readonly IBlobFactory blobFactory;

        protected readonly ContentMediaResolver mediaDataResolver;

        protected readonly IDatabaseExecutor databaseHandler;

        protected readonly ProjectRepository projectRepository;

        protected readonly CategoryRepository categoryRepository;

        protected readonly IContentSoftLinkRepository contentSoftLinkRepository;

        protected readonly IFrameRepository frameRepository;

        protected readonly IUrlSegmentGenerator urlSegmentGenerator;

        protected readonly IContentModelUsage contentModelUsage;

        protected readonly IParentRestoreRepository parentRestoreRepository;

        protected readonly IContentProviderManager contentProviderManager;

        protected readonly IPermanentLinkMapper permanentLinkMapper;

        public BlendJobBase()
        {
            var locator = ServiceLocator.Current;
            this.contentLoader = locator.GetInstance<IContentLoader>();
            this.contentRepository = locator.GetInstance<IContentRepository>();
            this.contentTypeRepository = locator.GetInstance<IContentTypeRepository>();
            this.siteDefinition = locator.GetInstance<SiteDefinition>();
            this.urlResolver = locator.GetInstance<UrlResolver>();
            this.blobFactory = locator.GetInstance<IBlobFactory>();
            this.mediaDataResolver = locator.GetInstance<ContentMediaResolver>();
            this.databaseHandler = locator.GetInstance<IDatabaseExecutor>();
            this.projectRepository = locator.GetInstance<ProjectRepository>();
            this.categoryRepository = locator.GetInstance<CategoryRepository>();
            this.contentSoftLinkRepository = locator.GetInstance<IContentSoftLinkRepository>();
            this.frameRepository = locator.GetInstance<IFrameRepository>();
            this.urlSegmentGenerator = locator.GetInstance<IUrlSegmentGenerator>();
            this.contentModelUsage = locator.GetInstance<IContentModelUsage>();
            this.parentRestoreRepository = locator.GetInstance<IParentRestoreRepository>();
            this.contentProviderManager = locator.GetInstance<IContentProviderManager>();
            this.permanentLinkMapper = locator.GetInstance<IPermanentLinkMapper>();
            this.IsStoppable = true;
        }

        protected string LogFilePath;

        protected int TotalRecords { get; set; }

        protected bool StopJob { get; set; }

        protected string CurrentStatus { get; set; }

        protected int CurrentRecordNumber { get; set; }

        protected Dictionary<string, int> Counters { get; set; }

        protected Stopwatch Stopwatch { get; set; }

        protected void StartTimer()
        {
            if (Stopwatch == null)
            {
                Stopwatch = new Stopwatch();
            }
            Stopwatch.Start();
        }

        protected void StopTimer()
        {
            Stopwatch.Stop();
        }

        public override void Stop()
        {
            StopJob = true;
            base.Stop();
        }

        protected void NewRecord()
        {
            CurrentRecordNumber++;
        }

        protected string ProgressString()
        {
            if (TotalRecords == 0)
            {
                return CurrentRecordNumber.ToString();
            }

            return String.Concat(CurrentRecordNumber, " of ", TotalRecords);
        }

        protected void SetStatus()
        {
            SetStatus(ProgressString());
        }

        protected void SetStatus(string currentStatus)
        {
            CurrentStatus = currentStatus;
        }

        protected void InitializeCounters(string counterNames)
        {
            Counters = new Dictionary<string, int>();
            foreach (string counterName in counterNames.Split(',').Select(s => s.Trim()))
            {
                Counters.Add(counterName, 0);
            }
        }

        protected void Increment(string counterName)
        {
            if (Counters == null)
            {
                Counters = new Dictionary<string, int>();
            }

            if (!Counters.ContainsKey(counterName))
            {
                Counters.Add(counterName, 0);
            }

            Counters[counterName]++;
        }

        protected void SetCounter(string counterName, int value)
        {
            if (Counters == null)
            {
                Counters = new Dictionary<string, int>();
            }

            if (!Counters.ContainsKey(counterName))
            {
                Counters.Add(counterName, value);
                return;
            }

            Counters[counterName] = value;
        }

        protected string CounterReport()
        {
            if (Counters == null)
            {
                return "No Counters";
            }

            var reports = new List<string>();

            foreach (KeyValuePair<string, int> counter in Counters)
            {
                reports.Add(String.Format("{0}: {1}", counter.Key, counter.Value));
            }

            string timeReport = String.Empty;
            if (Stopwatch != null)
            {
                timeReport = " in " + (Stopwatch.ElapsedMilliseconds / 1000) + " seconds";
            }

            return String.Join(", ", reports) + timeReport;
        }

        protected IEnumerable<PageData> GetAllPages()
        {
            var contentReferences = this.contentLoader.GetDescendents(siteDefinition.StartPage);
            return this.contentLoader.GetItems(contentReferences, new LoaderOptions())
                .OfType<PageData>();
        }

        protected IEnumerable<T> GetAllPages<T>() where T : ContentData
        {
            var contentReferences = this.contentLoader.GetDescendents(siteDefinition.StartPage);
            return this.contentLoader.GetItems(contentReferences, new LoaderOptions())
                .OfType<T>();
        }

        protected IEnumerable<T> GetAllPages<T>(ContentReference startReference, bool includeUnpublished) where T : ContentData
        {
            Debug.Assert(startReference.HasValue());
            var contentReferences = this.contentLoader.GetDescendents(startReference);
            if (includeUnpublished)
                return this.contentLoader.GetItems(contentReferences, LanguageSelector.AutoDetect(true)).OfType<T>();

            return this.contentLoader.GetItems(contentReferences, LanguageSelector.AutoDetect())
                .OfType<T>();
        }

        protected IEnumerable<ContentReference> GetAllPageReferences()
        {
            return this.contentLoader.GetDescendents(this.siteDefinition.StartPage);
        }

        protected IEnumerable<PageData> GetAllPagesForEdit(ContentReference startReference)
        {
            var contentReferences = this.contentLoader.GetDescendents(startReference);
            var pages = this.contentLoader.GetItems(contentReferences, new LoaderOptions())
                .Cast<PageData>();
            return pages.Select(x => x.CreateWritableClone());
        }
    }

    public class LogFile
    {
        private string internalPath;

        private readonly string fileNameFormat = "yyyy-MM-dd-hhmm";

        private readonly string fileExtension = "txt";

        public bool IncludeTimestamp { get; set; }

        public LogFile(string path)
        {
            InitializeLog(path);
            File.Delete(internalPath);
        }

        private void InitializeLog(string path)
        {
            if (path.StartsWith("~") || path.StartsWith("/"))
            {
                // Path is relative to the Web root
                path = String.Concat(AppDomain.CurrentDomain.BaseDirectory, path.TrimStart(@"~\/".ToCharArray())).Replace("/", @"\");
            }

            // Ensure a directory exists for this path
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // If this is not to an actual file, tack on a date-based filename
            if (Path.GetExtension(path) == String.Empty)
            {
                path = Path.Combine(path, String.Concat(DateTime.Now.ToString(fileNameFormat), ".", fileExtension));
            }

            internalPath = path;
        }

        public string Write()
        {
            return Write(String.Empty);
        }

        public string Write(string text)
        {
            return Write(text, new object[0]);
        }

        public string Write(object objectText, params object[] variables)
        {
            if (String.IsNullOrWhiteSpace(internalPath))
            {
                return String.Empty;
            }

            string text = objectText.ToString();

            if (variables.Count() > 0)
            {
                if (text.Contains("{0}"))
                {
                    text = String.Format(text, variables.Select(o => o.ToString()).ToArray());
                }
                else
                {
                    text = String.Concat(new List<string>() { text }.Concat(variables.Select(o => o.ToString()).ToArray()));
                }
            }

            if (text.Trim().Length > 0 && IncludeTimestamp)
            {
                text = DateTime.Now.ToShortTimeString() + ": " + text;
            }

            File.AppendAllLines(internalPath, new string[] { text });
            return text;
        }
    }

    public static class PageDataExtensions
    {
        public static bool SetValueIfDifferent(this PageData pageData, string propertyName, object compareValue)
        {
            if (pageData.Property[propertyName].Value != compareValue)
            {
                pageData.SetValue(propertyName, compareValue);
                return true;
            }
            return false;
        }

        public static PropertyDataCollection GetModifiedProperties(this PageData pageData)
        {
            var propDataCollection = new PropertyDataCollection();
            foreach (PropertyData propertyData in pageData.Property)
            {
                if (propertyData.IsModified)
                {
                    propDataCollection.Add(propertyData);
                }
            }

            return propDataCollection;
        }
    }

    public static class ObjectExtentions
    {
        public static string Quoted(this object theObject)
        {
            return String.Concat("\"", theObject.ToString(), "\"");
        }
    }
}