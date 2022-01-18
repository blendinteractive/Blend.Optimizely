using System;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;

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
            var descriptionAttributes = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false) as DisplayAttribute[];

            return (descriptionAttributes != null && descriptionAttributes.Length > 0) ? descriptionAttributes[0].Name : value.ToString();
        }
    }
}
