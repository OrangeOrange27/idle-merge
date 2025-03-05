using System;
using System.Collections.Generic;

namespace Common.Utils.Extensions
{
    public static class EnumExtensions
    {
        public static string ToUnderscoreCaseString(this Enum value)
        {
            var enumType = value.GetType();
            var enumName = Enum.GetName(enumType, value);
            var enumTypeNameUnderscore = enumType.Name.FromCamelToUnderscoreCase();
            var enumNameUnderscore = enumName.FromCamelToUnderscoreCase();
            return $"{enumTypeNameUnderscore}_{enumNameUnderscore}";
        }
        
        public static List<T> EnumToList<T>() where T : Enum
        {
            return new List<T>((T[])Enum.GetValues(typeof(T)));
        }
        
        public static int GetValuesCount<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}