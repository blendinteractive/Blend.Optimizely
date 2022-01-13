using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using System.Collections.Generic;
using System.Linq;

namespace Blend.Episerver
{
    public static class HasValueExtensions
    {
        public static bool HasValue(this ContentReference reference) => !ContentReference.IsNullOrEmpty(reference);

        public static bool HasValue(this ContentArea contentArea) => contentArea != null && contentArea.FilteredItems.Any();

        public static bool HasValue(this XhtmlString htmlString) => htmlString != null && !string.IsNullOrEmpty(htmlString.ToHtmlString());

        public static bool HasValue(this string value) => !string.IsNullOrEmpty(value);

        public static bool HasValue(this Url url) => url != null && !url.IsEmpty() && (url.Path.HasValue() || url.Scheme.HasValue());

        public static bool HasValue(this LinkItemCollection linkCollection) => linkCollection != null && linkCollection.Any();

        public static bool HasValue<T>(this IEnumerable<T> array) => array != null && array.Any();

        public static bool HasValue(this LinkItem item) => item != null && item.Text.HasValue() && item.Href.HasValue();

        public static ContentReference Coalesce(this ContentReference value, ContentReference alternative) => value.HasValue() ? value : alternative;

        public static XhtmlString Coalesce(this XhtmlString value, XhtmlString alternative) => value.HasValue() ? value : alternative;

        public static XhtmlString Coalesce(this XhtmlString value, string alternative) => value.HasValue() ? value : new XhtmlString(alternative);

        public static T Coalesce<T>(this T? value, T alternative) where T : struct => value.HasValue ? value.Value : alternative;

        public static string Coalesce(this string value, string alternative) => value.HasValue() ? value : alternative;
    }
}