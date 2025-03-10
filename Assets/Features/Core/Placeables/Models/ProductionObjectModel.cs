using System;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class ProductionObjectModel : PlaceableModel
    {
        public ProductionType ProductionType;
        public GameplayReactiveProperty<DateTime> NextCollectionDateTime = new();

        public ProductionObjectModel()
        {
        }

        protected ProductionObjectModel(ProductionObjectModel other) : base(other)
        {
            ProductionType = other.ProductionType;
            NextCollectionDateTime = new GameplayReactiveProperty<DateTime>(other.NextCollectionDateTime.Value);
        }

        public override PlaceableModel Clone()
        {
            return new ProductionObjectModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            NextCollectionDateTime?.Dispose();
        }
    }
}