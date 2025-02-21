using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
    public static class DictionaryExtensions
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
    }
}