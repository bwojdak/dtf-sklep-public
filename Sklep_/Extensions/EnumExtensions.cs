using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Sklep_.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var display = field?.GetCustomAttribute<DisplayAttribute>();
        return display?.GetName() ?? value.ToString();
    }
}