using System;
using Features.Core.ProductionSystem.Models;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class ProductionObjectModel : PlaceableModel
    {
        public ProductionType ProductionType;
        public ProductionConfig ProductionConfig;
        public RecycleResult RecycleResult;

        public GameplayReactiveProperty<DateTime> NextCollectionDateTime = new();
        public GameplayReactiveProperty<int> TimesCollected = new();

        public bool IsExausted => TimesCollected.Value >= ProductionConfig.MaximumCollectionTimes;

        public ProductionObjectModel()
        {
        }

        protected ProductionObjectModel(ProductionObjectModel other) : base(other)
        {
            ProductionType = other.ProductionType;
            ProductionConfig = other.ProductionConfig;
            RecycleResult = other.RecycleResult;
            NextCollectionDateTime = new GameplayReactiveProperty<DateTime>(other.NextCollectionDateTime.Value);
            TimesCollected = new GameplayReactiveProperty<int>(other.TimesCollected.Value);
        }

        public override PlaceableModel Clone()
        {
            return new ProductionObjectModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            NextCollectionDateTime?.Dispose();
            TimesCollected?.Dispose();
        }
    }
}