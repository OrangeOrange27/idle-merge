using System;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class CollectibleModel : PlaceableModel
    {
        public CollectibleType CollectibleType;

        public CollectibleModel()
        {
        }

        protected CollectibleModel(CollectibleModel other) : base(other)
        {
            CollectibleType = other.CollectibleType;
        }
        
        public override PlaceableModel Clone()
        {
            return new CollectibleModel(this);
        }
    }
}