using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Blend.Optimizely
{
    public static class EnumExtensions
    {
        static ConcurrentDictionary<Enum, string> cache = new ConcurrentDictionary<Enum, string>();
        public static string GetDisplayName(this Enum value) => cache.GetOrAdd(value, _GetDisplayName);
        public static string GetDisplayName<TEnum>(this object value) where TEnum : Enum => cache.GetOrAdd((TEnum)value, _GetDisplayName);
        private static string _GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            if (fieldInfo is null)
                throw new InvalidOperationException($"Could not get Enum value for {value}");

            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];
            if (descriptionAttributes.HasValue())
                return descriptionAttributes.First().Name ?? value.ToString();

            return value.ToString();
        }
    }
}
