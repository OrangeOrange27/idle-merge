using System.Linq;

namespace Common.Utils.Extensions
{
    public static class StringExtensions
    {
        public static string FromCamelToUnderscoreCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                .ToLower();
        }

        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str);
        }
    }
}