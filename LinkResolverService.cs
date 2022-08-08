﻿using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.SpecializedProperties;
using EPiServer.Web;
using EPiServer.Web.Routing;
using System;

namespace Blend.Optimizely
{
    [ServiceConfiguration]
    public class LinkResolverService
    {

        private Injected<IContentLoader> ContentLoader { get; }
        private Injected<ISiteDefinitionResolver> SiteDefinitionResolver { get; }
        private Injected<IUrlResolver> UrlResolver { get; }


        public virtual ResolvedLink? TryResolveLink(object? contentLink, LinkOptions options = LinkOptions.None)
        {
            if (contentLink is null)
                return null;

            var resolvedLink = contentLink switch
            {
                IResolvable resolvable => resolvable.Resolve(options),
                IContent content => ResolveIContent(content, options),
                Url url => ResolveUrl(url, options),
                LinkItem linkItem => ResolveLinkItem(linkItem, options),
                ContentReference contentReference => ResolveContentReference(contentReference, options),
                int integerReference => ResolveContentReference(new ContentReference(integerReference), options),
                _ => null
            };

            return resolvedLink;
        }

        public virtual ResolvedLink? ResolveLinkItem(LinkItem linkItem, LinkOptions options = LinkOptions.None)
        {
            var content = UrlResolver.Service.Route(new UrlBuilder(linkItem.Href));
            if (content is not null)
            {
                var resolvedContent = ResolveIContent(content, options);
                if (resolvedContent is not null)
                {
                    if (!string.IsNullOrEmpty(linkItem.Target))
                        return resolvedContent with { Target = linkItem.Target };
                    return resolvedContent;
                }
            }

            return new ResolvedLink(linkItem.Href, linkItem.Target);
        }

        public virtual ResolvedLink? ResolveUrl(Url url, LinkOptions options = LinkOptions.None)
        {
            var content = UrlResolver.Service.Route(new UrlBuilder(url));
            if (content is null)
                return new ResolvedLink(url.ToString(), null);

            return ResolveIContent(content, options);
        }

        public virtual ResolvedLink? ResolveIContent(IContent content, LinkOptions options = LinkOptions.None)
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


                href = ResolvePagedataHref(pageData, options);
            }
            else
            {
                href = UrlResolver.Service.GetUrl(content);

                if (options.HasFlag(LinkOptions.IncludeDomain))
                {
                    href = EnsureAbsoluteUrl(content.ContentLink, href);
                }
            }

            return new ResolvedLink(href, target);
        }

        protected virtual string? ResolvePagedataHref(PageData pageData, LinkOptions options)
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
                                return ResolvePagedataHref(shortcutPage, options);
                            }
                        }
                    }
                    break;
            }

            string href = UrlResolver.Service.GetUrl(pageData);

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

            return ResolveIContent(content, options);
        }

    }

}
