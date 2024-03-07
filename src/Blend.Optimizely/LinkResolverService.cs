using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Web;

namespace Blend.Optimizely
{
    [ServiceConfiguration]
    public class LinkResolverService
    {
        private Injected<IContentLoader> ContentLoader { get; }

        private Injected<ISiteDefinitionResolver> SiteDefinitionResolver { get; }

        private Injected<IUrlResolver> UrlResolver { get; }

        public virtual ResolvedLink? TryResolveLink(object? contentLink, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            if (contentLink is null)
                return null;

            var resolvedLink = contentLink switch
            {
                IResolvable resolvable => resolvable.Resolve(options),
                IContent content => ResolveIContent(content, options, languageBranchId),
                Url url => ResolveUrl(url, options),
                LinkItem linkItem => ResolveLinkItem(linkItem, options),
                ContentReference contentReference => ResolveContentReference(contentReference, options),
                int integerReference => ResolveContentReference(new ContentReference(integerReference), options),
                string href => ResolveUrlString(href, options),
                _ => null
            };

            return resolvedLink;
        }

        public virtual ResolvedLink? ResolveLinkItem(LinkItem linkItem, LinkOptions options = LinkOptions.None)
        {
            var resolvedUrl = ResolveUrlString(linkItem.Href, options);
            if (resolvedUrl is null)
                return null;

            if (!string.IsNullOrEmpty(linkItem.Target))
                return resolvedUrl with { Target = linkItem.Target };

            return resolvedUrl;
        }

        public virtual ResolvedLink? ResolveUrlString(string href, LinkOptions options = LinkOptions.None)
        {
            var content = UrlResolver.Service.Route(new UrlBuilder(href));
            if (content is not null)
            {
                var resolvedContent = ResolveIContent(content, options: options);
                if (resolvedContent is not null)
                {
                    string remaining = ExtractRemainingUrl(href);
                    resolvedContent = resolvedContent.AddAdditional(remaining);
                    return resolvedContent;
                }
            }

            return new ResolvedLink(href, null);
        }

        private string ExtractRemainingUrl(string href)
        {
            var url = new UrlBuilder(href);
            string remaining = "";
            if (url.QueryCollection is not null)
            {
                var epsremainingpath = url.QueryCollection["epsremainingpath"];
                if (!string.IsNullOrEmpty(epsremainingpath))
                {
                    remaining = epsremainingpath;
                }

                bool anyParameters = false;
                foreach (string key in url.QueryCollection)
                {
                    if (key is null || string.Compare(key, "epsremainingpath", true) == 0)
                        continue;
                    var value = url.QueryCollection[key];
                    string keyValuePair = $"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value ?? "")}";

                    if (!anyParameters)
                    {
                        remaining += "?";
                        anyParameters = true;
                    }
                    else
                    {
                        remaining += "&";
                    }
                    remaining += keyValuePair;
                }
            }

            if (!string.IsNullOrEmpty(url.Fragment))
            {
                remaining += url.Fragment;
            }

            return remaining;
        }

        public virtual ResolvedLink? ResolveUrl(Url url, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            var content = UrlResolver.Service.Route(new UrlBuilder(url));

            if (content is null)
            {
                return new ResolvedLink(url.ToString(), null);
            }

            return ResolveIContent(content, options: options, languageBranchId);
        }

        public virtual ResolvedLink? ResolveIContent(IContent content, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            string? target = null;
            string? href;

            if (content is PageData pageData)
            {
                PropertyFrame? targetFrame = pageData.Property["PageTargetFrame"] as PropertyFrame;
                if (targetFrame is not null)
                {
                    target = targetFrame.FrameName;
                }

                href = ResolvePagedataHref(pageData, options, languageBranchId);
            }
            else
            {
                href = string.IsNullOrWhiteSpace(languageBranchId) ?
                    UrlResolver.Service.GetUrl(content) :
                    UrlResolver.Service.GetUrl(content.ContentLink, languageBranchId);

                if (options.HasFlag(LinkOptions.IncludeDomain))
                {
                    href = EnsureAbsoluteUrl(content.ContentLink, href);
                }
            }

            return new ResolvedLink(href, target);
        }

        protected virtual string? ResolvePagedataHref(PageData pageData, LinkOptions options, string languageBranchId = "")
        {
            switch (pageData.LinkType)
            {
                case PageShortcutType.Inactive:
                    return null;

                case PageShortcutType.External:
                    return pageData.LinkURL;

                case PageShortcutType.Shortcut:
                    if (!options.HasFlag(LinkOptions.IgnoreInternalShortcuts))
                    {
                        var shortcutLink = pageData.Property["PageShortcutLink"]?.Value as ContentReference;
                        if (shortcutLink is not null)
                        {
                            if (ContentLoader.Service.TryGet(shortcutLink, out PageData shortcutPage))
                            {
                                return ResolvePagedataHref(shortcutPage, options, languageBranchId);
                            }
                        }
                    }
                    break;
            }

            string href = string.IsNullOrWhiteSpace(languageBranchId) ?
                UrlResolver.Service.GetUrl(pageData) :
                UrlResolver.Service.GetUrl(pageData.ContentLink, languageBranchId);

            if (options.HasFlag(LinkOptions.IncludeDomain))
            {
                href = EnsureAbsoluteUrl(pageData.ContentLink, href);
            }

            return href;
        }

        protected virtual string EnsureAbsoluteUrl(ContentReference contentReference, string href)
        {
            var uri = new Uri(href, UriKind.RelativeOrAbsolute);
            if (!uri.IsAbsoluteUri)
            {
                var siteDefinition = SiteDefinitionResolver.Service.GetByContent(contentReference, true);
                if (siteDefinition is not null)
                {
                    var urlBuilder = new UrlBuilder(href)
                    {
                        Scheme = siteDefinition.SiteUrl.Scheme,
                        Host = siteDefinition.SiteUrl.Host,
                        Port = siteDefinition.SiteUrl.Port
                    };

                    return urlBuilder.ToString();
                }
            }

            return uri.ToString();
        }

        public virtual ResolvedLink? ResolveContentReference(ContentReference contentReference, LinkOptions options = LinkOptions.None)
        {
            if (!ContentLoader.Service.TryGet(contentReference, out IContent content))
                return null;

            return ResolveIContent(content, options: options);
        }

        public virtual ResolvedLink? ResolveContentReference(ContentReference contentReference, LinkOptions options = LinkOptions.None, string languageBranchId = "")
        {
            if (!ContentLoader.Service.TryGet(contentReference, CultureInfo.GetCultureInfo(languageBranchId), out IContent content))
            {
                return null;
            }

            return ResolveIContent(content, options, languageBranchId);
        }
    }
}