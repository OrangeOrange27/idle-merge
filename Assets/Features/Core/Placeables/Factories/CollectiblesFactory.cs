using System;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public class CollectiblesFactory : IPlaceablesFactory<CollectibleType>
    {
        public PlaceableType FactoryType => PlaceableType.CollectibleObject;

        public PlaceableModel Create(CollectibleType type)
        {
            return new CollectibleModel() { ObjectType = PlaceableType.CollectibleObject, CollectibleType = type };
        }

        PlaceableModel IPlaceablesFactory.Create(Enum objectType)
        {
            return objectType is CollectibleType type
                ? Create(type)
                : throw new ArgumentException($"Invalid type {objectType}");
        }
    }
}