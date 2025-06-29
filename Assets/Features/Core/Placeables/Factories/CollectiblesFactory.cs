using System;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.Placeables.Factories
{
    public class CollectiblesFactory : IPlaceablesFactory<CollectibleType>
    {
        public PlaceableType FactoryType => PlaceableType.CollectibleObject;

        public PlaceableModel Create(CollectibleType type)
        {
            return new CollectibleModel()
            {
                ObjectType = PlaceableType.CollectibleObject,
                CollectibleType = type,
                Size = new Vector2(1,1) //todo: pass cfg
            };
        }

        PlaceableModel IPlaceablesFactory.Create(Enum objectType)
        {
            return objectType is CollectibleType type
                ? Create(type)
                : throw new ArgumentException($"Invalid type {objectType}");
        }
    }
}