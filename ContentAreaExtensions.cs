using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;

namespace Blend.Optimizely
{
    public static class ContentAreaExtensions
    {
        private static Injected<IContentLoader> ContentLoader;

        public static List<T> AsContent<T>(this ContentArea contentArea) where T : IContentData
        {
            var list = new List<T>();
            var items = contentArea.HasValue() ?
                contentArea.FilteredItems.Select(x => x.ContentLink) :
                Enumerable.Empty<ContentReference>();
            if (items.HasValue())
            {
                foreach (var item in items)
                {
                    if (ContentLoader.Service.TryGet(item, out T contentItem))
                    {
                        list.Add(contentItem);
                    }
                }
            }
            return list;
        }

        public static List<IContent> AsContent(this ContentArea contentArea) => AsContent<IContent>(contentArea);
    }
}