using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace Blend.Optimizely
{
    public static class ContentExtensions
    {
        private static Injected<IContentLoader> contentLoader { get; set; }

        /// <summary>
        /// Gets UrlResolver.Current.GetUrl(page.ContentLink), but with an additional action.
        /// So /en/login becomes /en/login/submit.  Useful for form action URLs
        /// </summary>
        public static string GetUrlWithAction<T>(this T page, string actionName) where T : IContent, ILocalizable
            => UrlResolver.Current.GetUrl(page.ContentLink, page.Language.Name, new VirtualPathArguments
            {
                RouteValues = new RouteValueDictionary(new { action = actionName }),
                // Work around for bug: https://world.optimizely.com/forum/developer-forum/CMS/Thread-Container/2020/12/iurlresolver-geturl-pages-with-custom-action-cache-issue/#245539
                // Epi doesn't take `action` from route dictionary into account when caching URLs
                // It does take `Action` as a property of RouteValueDictionary into account, though
                Action = actionName
            });

        /// <summary>
        /// Gets UrlResolver.Current.GetUrl(page.ContentLink), but with an additional action.
        /// So /en/login becomes /en/login/submit.  Useful for form action URLs
        /// </summary>
        public static string GetUrlWithAction(this ContentReference link, string actionName, string language = null)
            => UrlResolver.Current.GetUrl(link, language, new VirtualPathArguments
            {

                RouteValues = new RouteValueDictionary(new { action = actionName }),
                // Work around for bug: https://world.optimizely.com/forum/developer-forum/CMS/Thread-Container/2020/12/iurlresolver-geturl-pages-with-custom-action-cache-issue/#245539
                // Epi doesn't take `action` from route dictionary into account when caching URLs
                // It does take `Action` as a property of RouteValueDictionary into account, though
                Action = actionName
            });


        /// <summary>
        /// Recursively looks for the first ancestor with matching type.
        /// </summary>
        public static T FindFirstAncestorOfType<T>(this IContent instance) where T : IContent
        {
            if (instance is T)
                return (T)instance;

            return contentLoader.Service.GetAncestors(instance.ContentLink).OfType<T>().FirstOrDefault();
        }

        /// <summary>
        /// Recursively looks for the last ancestor with matching type.
        /// </summary>
        public static T FindLasttAncestorOfType<T>(this IContent instance) where T : IContent
        {
            return contentLoader.Service.GetAncestors(instance.ContentLink).OfType<T>().LastOrDefault();
        }
    }
}