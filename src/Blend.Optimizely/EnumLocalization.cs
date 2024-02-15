using EPiServer.Framework.Localization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Blend.Optimizely
{
    public static class EnumLocalization
    {
        public static string GetValueName<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var staticName = Enum.GetName(typeof(TEnum), value);
            if (staticName is null)
                throw new InvalidOperationException("Could not get name for enum value");

            string localizationPath = $"/enums/{typeof(TEnum).Name.ToLowerInvariant()}/{staticName.ToLowerInvariant()}";

            string localizedName;
            if (LocalizationService.Current.TryGetString(localizationPath, out localizedName))
            {
                return localizedName;
            }

            string descriptionValue = GetEnumDescription(value);

            return descriptionValue.Coalesce(staticName);
        }

        public static string GetValueName<TEnum>(object value) where TEnum : struct, Enum
        {
            return GetValueName((TEnum)value);
        }

        public static string GetEnumDescription<TEnum>(TEnum value) where TEnum : struct, Enum
        {
            var type = value.GetType();
            if (type is null || !type.IsEnum)
                throw new InvalidOperationException("Invalid TEnum type");
            var enumValue = value.ToString();
            if (enumValue is null)
                throw new InvalidOperationException("Invalid TEnum value");

            FieldInfo? fi = type.GetField(enumValue);
            if (fi is null)
                throw new InvalidOperationException($"Could not get FieldInfo for {enumValue}");

            DescriptionAttribute[]? attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return enumValue;
        }
    }
}