using System;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class Collectible
    {
        public CollectibleType CollectibleType;
        public int Amount;
    }
}