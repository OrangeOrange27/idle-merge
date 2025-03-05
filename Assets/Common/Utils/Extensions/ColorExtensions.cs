using UnityEngine;

namespace Common.Utils.Extensions
{
    public static class ColorExtensions
    {
        public static Color WithAlpha(this Color color, float alpha)
        {
            color.a = alpha;
            return color;
        }
    }
}