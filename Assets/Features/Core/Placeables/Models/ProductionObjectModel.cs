using System;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class ProductionObjectModel : PlaceableModel
    {
        public ProductionType ProductionType;
        public GameplayReactiveProperty<DateTime> NextCollectionDateTime = new();

        public override void Dispose()
        {
            base.Dispose();
            NextCollectionDateTime?.Dispose();
        }
    }
}