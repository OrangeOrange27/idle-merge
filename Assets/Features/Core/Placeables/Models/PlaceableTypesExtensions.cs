using System;

namespace Features.Core.Placeables.Models
{
    public static class PlaceableTypesExtensions
    {
        public static ProductionType ToProductionType(this CollectibleType collectibleType)
        {
            return collectibleType switch
            {
                CollectibleType.Fish => ProductionType.Fish,
                CollectibleType.Herbs => ProductionType.Herbs,
                CollectibleType.Fur => ProductionType.Fur,
                CollectibleType.Wood => ProductionType.Wood,
                CollectibleType.Feather => ProductionType.Feather,
                CollectibleType.Essence => ProductionType.Essence,
                CollectibleType.Dust => ProductionType.Dust,
                CollectibleType.ToyParts => ProductionType.ToyParts,
                CollectibleType.Crystal => ProductionType.Crystal,
                CollectibleType.Milk => ProductionType.Milk,
                _ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
            };
        }

        public static CollectibleType ToCollectibleType(this ProductionType collectibleType)
        {
            return collectibleType switch
            {
                ProductionType.Fish => CollectibleType.Fish,
                ProductionType.Herbs => CollectibleType.Herbs,
                ProductionType.Fur => CollectibleType.Fur,
                ProductionType.Wood => CollectibleType.Wood,
                ProductionType.Feather => CollectibleType.Feather,
                ProductionType.Essence => CollectibleType.Essence,
                ProductionType.Dust => CollectibleType.Dust,
                ProductionType.ToyParts => CollectibleType.ToyParts,
                ProductionType.Crystal => CollectibleType.Crystal,
                ProductionType.Milk => CollectibleType.Milk,
                _ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
            };
        }
    }
}