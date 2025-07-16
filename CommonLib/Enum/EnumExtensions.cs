using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace CommonLib.Enum
{
    public static class EnumExtensions
    {
        public static string GetDisplayName(this System.Enum value)
        {
            var type = value.GetType();
            var name = System.Enum.GetName(type, value);

            if (name != null)
            {
                FieldInfo field = type.GetField(name);

                if (field != null)
                {
                    var displayAttribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;

                    if (displayAttribute != null)
                    {
                        return displayAttribute.Name;
                    }
                }
            }

            throw new Exception("Enum DisplayName değeri alınamadı!");
        }
        public static TEnum GetValueFromDisplayName<TEnum>(string displayName) where TEnum : struct
        {
            var type = typeof(TEnum);

            if (!type.IsEnum)
                throw new ArgumentException("TEnum must be an enumerated type");

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;

                if (attribute != null && attribute.Name == displayName)
                {
                    return (TEnum)field.GetValue(null);
                }
            }

            throw new ArgumentException($"'{displayName}' adında bir Display değeri {typeof(TEnum).Name} içinde bulunamadı.");
        }
    }
}