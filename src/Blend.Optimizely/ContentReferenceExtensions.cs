﻿using EPiServer;
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

        private static string? GetFetchDataFriendlyUrl(ContentReference contentReference, GetFriendlyUrlOption options, string? actionName, string? languageBranchId)
        {
            PageData asContent;
            try
            {
                if (!languageBranchId.HasValue())
                    asContent = contentLoader.Service.Get<PageData>(contentReference);
                else
                    asContent = contentLoader.Service.Get<PageData>(contentReference, CultureInfo.GetCultureInfo(languageBranchId));
            }
            catch (ContentNotFoundException)
            {
                return null;
            }

            if (asContent.LinkType != PageShortcutType.FetchData)
                return null;

            var pageShortcutLink = asContent.Property["PageShortcutLink"].ToString();
            if (!int.TryParse(pageShortcutLink, out int shortcutContentId))
                return null;

            return GetFriendlyUrl(new ContentReference(shortcutContentId), options, actionName, languageBranchId);
        }

        /// <summary>
        /// Returns friendly URL for provided content reference.
        /// </summary>
        /// <param name="contentReference">Content reference for which to create friendly url.</param>
        /// <param name="includeHost">Mark if include host name in the url.</param>
        /// <returns>String representation of URL for provided content reference.</returns>
        [Obsolete("Use ResolveUrl extension method instead")]
        public static string GetFriendlyUrl(this ContentReference contentReference, GetFriendlyUrlOption options = GetFriendlyUrlOption.None, string? actionName = null, string? languageBranchId = null)
        {
            if (!contentReference.HasValue()) return string.Empty;

            if ((options & GetFriendlyUrlOption.FollowShortcuts) != 0)
            {
                var fetchedUrl = GetFetchDataFriendlyUrl(contentReference, options, actionName, languageBranchId);
                if (fetchedUrl != null)
                {
                    return fetchedUrl;
                }
            }

            string url;
            if (actionName == null)
            {
                url = !languageBranchId.HasValue() ?
                    urlResolver.Service.GetUrl(contentReference) :
                    urlResolver.Service.GetUrl(contentReference, languageBranchId);
            }
            else
            {
                url = contentReference.GetUrlWithAction(actionName, languageBranchId);
            }

            return GetFriendlyUrl(url, options);
        }

        public static string GetNonContentFriendlyUrl(object nonConentObject, GetFriendlyUrlOption options = GetFriendlyUrlOption.None, string? language = null, VirtualPathArguments? virtualPathArguments = null)
        {
            var urlResolver = ServiceLocator.Current.GetInstance<UrlResolver>();
            var url = urlResolver.GetVirtualPathForNonContent(nonConentObject, language, virtualPathArguments).GetUrl();
            return GetFriendlyUrl(url, options);
        }

        [Obsolete("Use LinkResolverService instead")]
        public static string GetFriendlyUrl(string url, GetFriendlyUrlOption options = GetFriendlyUrlOption.None)
        {
            bool useSiteDefinitionHost = (options & GetFriendlyUrlOption.UseSiteDefinitionHost) != 0;
            if (useSiteDefinitionHost)
                options |= GetFriendlyUrlOption.IncludeHost;

            bool includeHost = (options & GetFriendlyUrlOption.IncludeHost) != 0;
            bool forceHttps = ((options & GetFriendlyUrlOption.ForceHttps) != 0);

            if (!includeHost && !forceHttps)
            {
                return url;
            }

            Uri siteUri = SiteDefinition.Current.SiteUrl;

            var urlBuilder = new UrlBuilder(url)
            {
                Scheme = siteUri.Scheme,
                Host = siteUri.Host,
                Port = siteUri.Port
            };
            if (forceHttps)
            {
                urlBuilder.Port = 443;
                urlBuilder.Scheme = Uri.UriSchemeHttps;
            }
            return urlBuilder.ToString();
        }

        [Obsolete("Use ResolveUrl extension method instead")]
        public static string GetFriendlyUrl(this PageData pageData, GetFriendlyUrlOption options = GetFriendlyUrlOption.None) => GetFriendlyUrl(pageData.ContentLink, options);

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

            var page = contentLink.Get<PageData>();
            var rootPage = SiteDefinition.Current.RootPage;
            var startPage = SiteDefinition.Current.StartPage;

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