using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace WebApp.Data.enums;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        var enumMember = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault();

        if (enumMember != null)
        {
            var displayAttribute = enumMember.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null && displayAttribute.Name != null) return displayAttribute.Name;
        }

        // Fallback to enum name if no Display attribute is found
        return enumValue.ToString();
    }
}