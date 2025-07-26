using System;

namespace Common.Data
{
    [Serializable]
    public class WeightedEntry<T>
    {
        public T Item;
        public float Weight;
    }
}