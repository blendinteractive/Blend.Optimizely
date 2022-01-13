using EPiServer.Framework.Localization;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Blend.Optimizely
{
    public static class EnumLocalization
    {
        public static string GetValueName<TEnum>(TEnum value)
        {
            var staticName = Enum.GetName(typeof(TEnum), value);

            string localizationPath = $"/enums/{typeof(TEnum).Name.ToLowerInvariant()}/{staticName.ToLowerInvariant()}";

            string localizedName;
            if (LocalizationService.Current.TryGetString(localizationPath, out localizedName))
            {
                return localizedName;
            }

            string descriptionValue = GetEnumDescription(value);

            return descriptionValue.Coalesce(staticName);
        }

        public static string GetValueName<TEnum>(object value)
        {
            return GetValueName((TEnum)value);
        }

        public static string GetEnumDescription<TEnum>(TEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}