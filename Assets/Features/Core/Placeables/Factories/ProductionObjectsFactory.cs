using System;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public class ProductionObjectsFactory : IPlaceablesFactory<ProductionType>
    {
        public PlaceableType FactoryType => PlaceableType.ProductionEntity;

        public PlaceableModel Create(ProductionType type)
        {
            return new ProductionObjectModel() { ObjectType = PlaceableType.ProductionEntity, ProductionType = type };
        }

        public PlaceableModel Create(Enum objectType)
        {
            return objectType is ProductionType type
                ? Create(type)
                : throw new ArgumentException($"Invalid type {objectType}");
        }
    }
}