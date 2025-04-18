using System;

namespace Features.Core.Placeables.Models
{
    public static class PlaceableTypesExtensions
    {
        public static ProductionType ToProductionType(this CollectibleType collectibleType)
        {
            return collectibleType switch
            {
                CollectibleType.None => ProductionType.None,
                CollectibleType.Wheat => ProductionType.Wheat,
                CollectibleType.Milk => ProductionType.Milk,
                CollectibleType.Egg => ProductionType.Egg,
                CollectibleType.Carrot => ProductionType.Carrot,
                _ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
            };
        }

        public static CollectibleType ToCollectibleType(this ProductionType collectibleType)
        {
            return collectibleType switch
            {
                ProductionType.None => CollectibleType.None,
                ProductionType.Wheat => CollectibleType.Wheat,
                ProductionType.Milk => CollectibleType.Milk,
                ProductionType.Egg => CollectibleType.Egg,
                ProductionType.Carrot => CollectibleType.Carrot,
                _ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
            };
        }
    }
}