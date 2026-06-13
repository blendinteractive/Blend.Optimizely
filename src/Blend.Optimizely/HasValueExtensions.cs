using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Blend.Optimizely
{
    /// <summary>
    /// A collection of extension methods to check if a property has a value. Checks vary depending on what the object is.
    /// </summary>
    public static class HasValueExtensions
    {
        public static bool HasValue([NotNullWhen(true)] this  ContentReference? reference) => !ContentReference.IsNullOrEmpty(reference);

        public static bool HasValueUnfiltered([NotNullWhen(true)] this ContentArea? contentArea) => contentArea != null && contentArea.Items.Any();

        [Obsolete("This calls `HasValueUnfiltered` which may not be what you want. Instead, use AsContentAsync or GetFilteredItemsAsync first, then check the results of that.")]
        public static bool HasValue([NotNullWhen(true)] this ContentArea? contentArea) => contentArea != null && contentArea.Items.Any();

        public static bool HasValue([NotNullWhen(true)] this XhtmlString? htmlString) => htmlString != null && !string.IsNullOrEmpty(htmlString.ToHtmlString());

        public static bool HasValue([NotNullWhen(true)] this string? value) => !string.IsNullOrEmpty(value);

        public static bool HasValue([NotNullWhen(true)] this Url? url) => url != null && !url.IsEmpty() && (url.Path.HasValue() || url.Scheme.HasValue());

        public static bool HasValue([NotNullWhen(true)] this LinkItemCollection? linkCollection) => linkCollection != null && linkCollection.Any();

        public static bool HasValue<T>([NotNullWhen(true)] this IEnumerable<T>? array) => array != null && array.Any();

        public static bool HasValue([NotNullWhen(true)] this LinkItem? item) => item != null && item.Text.HasValue() && item.Href.HasValue();

        public static bool HasValue([NotNullWhen(true)] this object? value) => value switch
        {
            ContentReference contentReference => contentReference.HasValue(),
            ContentArea contentArea => contentArea.HasValueUnfiltered(),
            XhtmlString xhtmlString => xhtmlString.HasValue(),
            string stringValue => stringValue.HasValue(),
            Url url => url.HasValue(),
            LinkItemCollection linkItemCollection => linkItemCollection.HasValue(),
            IEnumerable iEnumerable => iEnumerable.HasValue(),
            LinkItem linkItem => linkItem.HasValue(),
            _ => value != null
        };

        public static ContentReference Coalesce(this ContentReference? value, ContentReference alternative) => value.HasValue() ? value : alternative;

        public static XhtmlString Coalesce(this XhtmlString? value, XhtmlString alternative) => value.HasValue() ? value : alternative;

        public static XhtmlString Coalesce(this XhtmlString? value, string alternative) => value.HasValue() ? value : new XhtmlString(alternative);

        public static T Coalesce<T>(this T? value, T alternative) where T : struct => value.HasValue ? value.Value : alternative;

        public static string Coalesce(this string? value, string alternative) => value.HasValue() ? value : alternative;
    }
}