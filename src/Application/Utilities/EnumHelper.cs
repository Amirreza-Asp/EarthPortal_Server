using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Application.Utilities
{
    public static class EnumHelper
    {
        public static string GetEnumDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            return attribute?.Name ?? value.ToString();
        }
    }
}
