using System;
using System.Collections.Generic;

namespace Common.Utils.Extensions
{
    public static class ListExtensions
    {
        private static readonly Random _random = new();

        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng = new();
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
        
        public static void Shuffle<T>(this IList<T> list, int seed)
        {
            Random rng = new(seed);
            int n = list.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
        
        public static T GetRandomElement<T>(this List<T> list)
        {
            int randomIndex = _random.Next(0, list.Count);
            return list[randomIndex];
        }
        
        public static List<int> GenerateRandomNumbersList(int size)
        {
            Random random = new Random();
            List<int> numbers = new(size);

            for (int i = 0; i < size; i++)
            {
                numbers.Add(random.Next(1000, 10001));
            }

            return numbers;
        }
        
        public static List<int> GenerateRandomSeedList(int size, int min, int max)
        {
            List<int> randomList = new List<int>();

            for (int i = 0; i < size; i++)
            {
                randomList.Add(_random.Next(min, max + 1));
            }

            return randomList;
        }
    }
}