using System;

namespace Features.Core.Placeables.Models
{
    public enum CollectibleType
    {
        None = 0,
        Wheat,
        Milk,
        Egg,
        Carrot,
        //etc
    }
    
    [Serializable]
    public class CollectibleModel
    {
        public CollectibleType CollectibleType;
        public int Amount;
    }
}