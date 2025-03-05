using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Common.Utils
{
    public static class IEnumerableExtensions
    {
        public static KeyValuePair<TKey, TValue> GetRandom<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            var index = Random.Range(0, dictionary.Count);
            var i = 0;
            foreach (var pair in dictionary)
            {
                if (i == index)
                {
                    return pair;
                }

                i++;
            }

            return default;
        }
        
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable)
        {
            return enumerable == null || !enumerable.Any();
        }
    }
}