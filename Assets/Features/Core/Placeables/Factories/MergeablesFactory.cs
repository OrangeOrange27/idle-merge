using System;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public class MergeablesFactory : IPlaceablesFactory<MergeableType>
    {
        public PlaceableType FactoryType => PlaceableType.MergeableObject;

        public PlaceableModel Create(MergeableType type)
        {
            return new MergeableModel() { ObjectType = PlaceableType.MergeableObject, MergeableType = type };
        }

        PlaceableModel IPlaceablesFactory.Create(Enum objectType)
        {
            return objectType is MergeableType type
                ? Create(type)
                : throw new ArgumentException($"Invalid type {objectType}");
        }
    }
}