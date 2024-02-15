using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System.Linq;

namespace Blend.Optimizely
{
    public static class ContentAreaExtensions
    {
        private static Injected<IContentLoader> ContentLoader;

        /// <summary>
        /// Returns a list of content from a content area.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentArea"></param>
        /// <returns></returns>
        public static List<T> AsContent<T>(this ContentArea? contentArea) where T : IContentData
        {
            var list = new List<T>();
            if (contentArea is null)
                return list;

            foreach (var item in contentArea.FilteredItems)
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

        public static List<IContent> AsContent(this ContentArea? contentArea) => AsContent<IContent>(contentArea);
    }
}