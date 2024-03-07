using EPiServer.DataAbstraction;
using EPiServer.Framework.Web;
using EPiServer.Web.Mvc;
using System;

namespace Blend.Optimizely.Rendering
{
    public abstract class AbstractTemplateCoordinator : IViewTemplateModelRegistrator
    {
        public virtual string BlockFolder { get; } = "~/Views/Blocks/";

        public virtual string MediaFolder { get; } = "~/Views/Media/";

        public virtual string PagePartialsFolder { get; } = "~/Views/PagePartials/";

        public virtual string PagesFolder { get; } = "~/Views/Pages/";

        protected TemplateModelCollection? TemplateModelCollection { get; private set; }

        public void Register(TemplateModelCollection viewTemplateModelRegistrator)
        {
            this.TemplateModelCollection = viewTemplateModelRegistrator;
            Register();
        }

        protected abstract void Register();

        public static readonly string TemplatePathString = "{0}{1}/{2}.cshtml";

        protected virtual void RegisterBlock<T>(params string[] tags)
        {
            if (TemplateModelCollection is null)
                throw new InvalidOperationException("RegisterBlock must be called from Register");

            string typeName = typeof(T).Name;

            TemplateModelCollection.Add(typeof(T), new TemplateModel
            {
                Name = typeName,
                AvailableWithoutTag = true,
                Inherit = true,
                Path = string.Format(TemplatePathString, BlockFolder, typeName, "Default"),
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
            });

            foreach (string tag in tags)
            {
                TemplateModelCollection.Add(typeof(T), new TemplateModel
                {
                    Name = typeName + tag,
                    Tags = new string[] { tag },
                    AvailableWithoutTag = false,
                    Inherit = true,
                    Path = string.Format(TemplatePathString, BlockFolder, typeName, tag),
                    TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
                });
            }
        }

        protected virtual void RegisterPartial<T>(params string[] tags)
        {
            if (TemplateModelCollection is null)
                throw new InvalidOperationException("RegisterPartial must be called from Register");

            string typeName = typeof(T).Name;

            TemplateModelCollection.Add(typeof(T), new TemplateModel
            {
                Name = typeName,
                AvailableWithoutTag = true,
                Inherit = true,
                Path = string.Format(TemplatePathString, PagePartialsFolder, typeName, "Default"),
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
            });
            if (tags != null)
                this.RegisterPartialsOnly<T>(tags);
        }

        protected virtual void RegisterPartialsOnly<T>(params string[] tags)
        {
            if (TemplateModelCollection is null)
                throw new InvalidOperationException("RegisterPartialsOnly must be called from Register");

            string typeName = typeof(T).Name;

            foreach (string tag in tags)
            {
                TemplateModelCollection.Add(typeof(T), new TemplateModel
                {
                    Name = typeName + tag,
                    Tags = new string[] { tag },
                    AvailableWithoutTag = false,
                    Inherit = true,
                    Path = string.Format(TemplatePathString, PagePartialsFolder, typeName, tag),
                    TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
                });
            }
        }

        protected virtual void RegisterMedia<T>(params string[] tags)
        {
            if (TemplateModelCollection is null)
                throw new InvalidOperationException("RegisterMedia must be called from Register");

            string typeName = typeof(T).Name;

            TemplateModelCollection.Add(typeof(T), new TemplateModel
            {
                Name = typeName,
                AvailableWithoutTag = true,
                Inherit = true,
                Path = string.Format(TemplatePathString, MediaFolder, string.Empty, typeName),
                TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
            });

            foreach (string tag in tags)
            {
                TemplateModelCollection.Add(typeof(T), new TemplateModel
                {
                    Name = typeName + tag,
                    Tags = new string[] { tag },
                    AvailableWithoutTag = false,
                    Inherit = true,
                    Path = string.Format(TemplatePathString, MediaFolder, typeName, tag),
                    TemplateTypeCategory = TemplateTypeCategories.MvcPartialView
                });
            }
        }
    }
}