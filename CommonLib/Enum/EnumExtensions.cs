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
    }
}