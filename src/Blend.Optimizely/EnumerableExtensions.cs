﻿using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Blend.Optimizely
{
    public static class EnumerableExtensions
    {
        private static Injected<IContentLoader> ContentLoader { get; set; }

        /// <summary>
        /// Splits an enumerable into chunks. For example a list of 7 in chunk sizes of 2, would return 3 groups of 2, and 1 group of 1
        /// Also see Split
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="chunksize"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> source, int chunksize)
        {
            while (source.HasValue())
            {
                yield return source.Take(chunksize);
                source = source.Skip(chunksize);
            }
        }

        // Maintains Sort order.
        /// <summary>
        /// Splits an enumerable into a set number of groups, group size is automatically determined.
        /// Also See Chunk
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int parts)
        {
            var list = new List<T>(source);
            int defaultSize = (int)((double)list.Count / (double)parts);
            int offset = list.Count % parts;
            int position = 0;

            for (int i = 0; i < parts; i++)
            {
                int size = defaultSize;
                if (i < offset)
                {
                    size++; // Just add one to the size (it's enough).
                }

                yield return list.GetRange(position, size);

                // Set the new position after creating a part list, so that it always start with position zero on the first yield return above.
                position += size;
            }
        }

        /// <summary>
        /// Converts an Enumerable of Content references into an enumerable of content type T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="contentLinks"></param>
        /// <returns></returns>
        public static IEnumerable<T> AsContent<T>(this IEnumerable<ContentReference>? contentLinks) where T : IContentData
        {
            if (contentLinks is null)
                return Enumerable.Empty<T>();

            var list = new List<T>();
            foreach (var item in contentLinks)
            {
                if (ContentLoader.Service.TryGet(item, out T contentItem))
                {
                    list.Add(contentItem);
                }
            }
            return list;
        }

        public static void AddValuesToCollection(this NameValueCollection addTo, NameValueCollection addFrom)
        {
            foreach (string str in addFrom)
                AddValues(addTo, str, addFrom.GetValues(str));
        }

        public static void AddValuesToCollection(this NameValueCollection addTo, NameValueCollection addFrom, List<string> includeKeys, List<string> excludeKeys)
        {
            foreach (string str in addFrom)
            {
                if ((((includeKeys != null) && (includeKeys.Count > 0)) && includeKeys.Contains(str)) && ((excludeKeys == null) || !excludeKeys.Contains(str)))
                    AddValues(addTo, str, addFrom.GetValues(str));

                if (((includeKeys == null) || (includeKeys.Count < 1)) && ((excludeKeys == null) || !excludeKeys.Contains(str)))
                    AddValues(addTo, str, addFrom.GetValues(str));
            }
        }

        private static void AddValues(NameValueCollection addTo, string key, string[]? keyValues)
        {
            if (keyValues is null)
                return;

            for (int i = 0; i < keyValues.Length; i++)
                addTo.Add(key, keyValues[i]);
        }

        public static bool IsLast<T>(this IEnumerable<T> items, T item)
        {
            var last = items.LastOrDefault();
            if (last == null)
                return false;
            return last.Equals(item);
        }

        public static bool IsFirst<T>(this IEnumerable<T> items, T item)
        {
            var first = items.FirstOrDefault();
            if (first == null)
                return false;
            return first.Equals(item);
        }

        public static bool IsFirstOrLast<T>(this IEnumerable<T> items, T item) =>
            items.IsFirst(item) || items.IsLast(item);

        public static T? GetNext<T>(this IEnumerable<T> list, T current)
        {
            if (!list.HasValue())
                return default;
            try
            {
                return list.SkipWhile(x => !x!.Equals(current)).Skip(1).First();
            }
            catch
            {
                return default;
            }
        }

        public static T? GetPrevious<T>(this IEnumerable<T> list, T current)
        {
            if (!list.HasValue())
                return default;
            try
            {
                return list.TakeWhile(x => !x!.Equals(current)).Last();
            }
            catch
            {
                return default;
            }
        }

        public static void ForEach<T>(this IEnumerable<T>? source, Action<T> action)
        {
            if (source is null)
                return;

            foreach (var element in source)
                action(element);
        }
    }
}