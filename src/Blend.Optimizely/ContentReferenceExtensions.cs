using EPiServer;
using EPiServer.Applications;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Blend.Optimizely
{
    public static class ContentReferenceExtensions
    {
        private static Injected<IContentLoader> contentLoader;

        private static Injected<UrlResolver> urlResolver;

        private static Injected<SystemDefinition> systemDefinition;

        private static Injected<IApplicationResolver> applicationResolver;

        /// <summary>
        /// Gets the specific language of content item represented by the provided reference.
        /// </summary>
        /// <typeparam name="TContent">The type of content to get.</typeparam>
        /// <param name="contentLink">The link to the content.</param>
        /// <param name="language">The language</param>
        /// <returns>The type of content to get.</returns>
        public static TContent Get<TContent>(this ContentReference contentLink, string? language = null) where TContent : IContentData
        {
            if (language != null)
                return contentLoader.Service.Get<TContent>(contentLink, CultureInfo.GetCultureInfo(language));

            return contentLoader.Service.Get<TContent>(contentLink);
        }

        /// <summary>
        /// Gets the children of the content item represented by the provided reference given the language.
        /// </summary>
        /// <typeparam name="TContent">The type of children to get.</typeparam>
        /// <param name="contentLink">A reference to the parent whose children should be returned.</param>
        /// <param name="language">The language.</param>
        /// <returns>The children of the specifed parent, as the specified type.</returns>
        public static IEnumerable<TContent> GetChildren<TContent>(this ContentReference contentLink, string? language = null) where TContent : IContentData
        {
            if (language != null)
                return contentLoader.Service.GetChildren<TContent>(contentLink, CultureInfo.GetCultureInfo(language));

            return contentLoader.Service.GetChildren<TContent>(contentLink);
        }

        public static string? ResolveUrl(this LinkItem linkItem, LinkOptions options = LinkOptions.None)
        {
            var resolved = ServiceLocator.Current.GetInstance<LinkResolverService>().ResolveLinkItem(linkItem, options);
            if (resolved is null)
                return null;
            return resolved.Href;
        }

        public static string? ResolveUrl(this IContent content, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            var resolved = ServiceLocator.Current.GetInstance<LinkResolverService>().ResolveIContent(content, options, languageBranchId);
            if (resolved is null)
                return null;
            return resolved.Href;
        }

        public static string? ResolveUrl(this ContentReference content, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            var resolved = ServiceLocator.Current.GetInstance<LinkResolverService>().ResolveContentReference(content, options, languageBranchId);
            if (resolved is null)
                return null;
            return resolved.Href;
        }

        public static string? ResolveUrl(this Url url, LinkOptions options = LinkOptions.None, string languageBrachId = "")
        {
            var resolved = ServiceLocator.Current.GetInstance<LinkResolverService>().ResolveUrl(url, options, languageBrachId);
            if (resolved is null)
                return null;
            return resolved.Href;
        }

        /// <summary>
        /// Recursively looks for parent pages with matching PageTypName
        /// </summary>
        /// <typeparam name="TPageData"></typeparam>
        /// <param name="rootPage">The root contentReference in which to start the recursion.</param>
        /// <param name="includeRootPage">Includes the current root page to start instead of parent</param>
        /// <returns></returns>
        public static TPageData? FindFirstAncestorOfType<TPageData>(this ContentReference rootPage) where TPageData : PageData
        {
            var contentItem = contentLoader.Service.Get<IContent>(rootPage);
            if (contentItem is PageData)
            {
                return ((PageData)contentItem).FindFirstAncestorOfType<TPageData>();
            }
            return null;
        }

        public static ContentReference GetOrCreateAssetFolder(this ContentReference contentLink)
        {
            var contentAssetHelper = ServiceLocator.Current.GetInstance<ContentAssetHelper>();
            var assetsFolder = contentAssetHelper.GetOrCreateAssetFolder(contentLink);
            return assetsFolder.ContentLink;
        }

        /// <summary>
        /// Get the ancestor page directly below the start page.
        /// </summary>
        public static PageData GetAncestorBelowStart(this ContentReference contentLink)
        {
            if (!contentLink.HasValue())
                throw new NotSupportedException("Current top page cannot be retrieved without a starting point, and the specified page link was empty");

            if (!(applicationResolver.Service.GetByContext() is Website webapp))
                throw new NotSupportedException("GetAncestorBelowStart is currently only supported on Wesite applications");


            var page = contentLink.Get<PageData>();
            var rootPage = systemDefinition.Service.RootPage;
            var startPage = webapp.EntryPoint;

            while (page.ParentLink.HasValue() &&
                !page.ParentLink.CompareToIgnoreWorkID(rootPage) &&
                !page.ParentLink.CompareToIgnoreWorkID(startPage))
            {
                page = page.ParentLink.Get<PageData>();
            }

            return page;
        }

        public static string GetFriendlyUrl(this Url url)
        {
            return UrlResolver.Current.GetUrl(new UrlBuilder(url), EPiServer.Web.ContextMode.Default);
        }
    }

    [Flags]
    public enum GetFriendlyUrlOption
    {
        None = 0,

        IncludeHost = 1,

        ForceHttps = 1 << 1,

        UseSiteDefinitionHost = 1 << 2,

        FollowShortcuts = 1 << 3,

        Canonical = UseSiteDefinitionHost | FollowShortcuts
    }
}