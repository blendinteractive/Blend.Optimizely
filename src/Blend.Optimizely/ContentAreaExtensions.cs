using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Blend.Optimizely
{
    public static class ContentAreaExtensions
    {
        private static Injected<IContentLoader> ContentLoader;

        public static async ValueTask<IList<ContentAreaItem>> GetFilteredItemsAsync(this ContentArea? contentArea)
        {
            if (contentArea is null)
                return new List<ContentAreaItem>();

            var filters = ServiceLocator.Current.GetService<IEnumerable<IContentAreaItemsRenderingFilter>>();
            if (filters is null)
                return contentArea.Items;

            IPrincipal? principal = null;
            var httpContextAccessor = ServiceLocator.Current.GetService<IHttpContextAccessor>();
            var httpContext = httpContextAccessor?.HttpContext;
            if (httpContext is not null)
            {
                principal = httpContext.User;
            }
            else
            {
                principal = new ClaimsPrincipal(new ClaimsIdentity());
            }

            var list = new List<ContentAreaItem>(contentArea.Items);
            foreach (var filter in filters)
            {
                await filter.FilterAsync(list, principal, ContextMode.Default);
            }

            return list;
        }

        /// <summary>
        /// Returns an UNFILTERED list of content from a content area.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentArea"></param>
        /// <returns></returns>
        public static List<T> AsContentUnfiltered<T>(this ContentArea? contentArea) where T : IContentData
        {
            if (contentArea is null)
                return new List<T>();
            return LoadContentAreaItems<T>(contentArea.Items);
        }

        public static List<IContent> AsContentUnfiltered(this ContentArea? contentArea) => AsContentUnfiltered<IContent>(contentArea);


        /// <summary>
        /// Returns a filtered list of content from a content area.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentArea"></param>
        /// <returns></returns>
        public static async ValueTask<List<T>> AsContentAsync<T>(this ContentArea? contentArea) where T : IContentData
        {
            var filteredItems = await contentArea.GetFilteredItemsAsync();
            return LoadContentAreaItems<T>(filteredItems);
        }

        public static async ValueTask<List<IContent>> AsContentAsync(this ContentArea? contentArea) => await AsContentAsync<IContent>(contentArea);

        private static List<T> LoadContentAreaItems<T>(IList<ContentAreaItem> filteredItems) where T : IContentData
        {
            var list = new List<T>();
            foreach (var item in filteredItems)
            {
                if (item.ContentLink.HasValue())
                {
                    if (ContentLoader.Service.TryGet(item.ContentLink, out T contentItem))
                    {
                        list.Add(contentItem);
                    }
                }
                else
                {
                    if (item.InlineBlock is T contentData)
                    {
                        list.Add(contentData);
                    }
                }
            }
            return list;
        }
    }
}