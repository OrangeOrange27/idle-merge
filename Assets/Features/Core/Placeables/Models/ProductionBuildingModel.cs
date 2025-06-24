using System;
using Features.Core.ProductionSystem.Models;

namespace Features.Core.Placeables.Models
{
    public class ProductionBuildingModel : PlaceableModel
    {
        public ProductionRecipe[] Recipes;
        public GameplayReactiveProperty<DateTime> NextCollectionDateTime = new();
        
        public ProductionBuildingModel()
        {
        }

        protected ProductionBuildingModel(ProductionBuildingModel other) : base(other)
        {
            Recipes = other.Recipes;
            NextCollectionDateTime = new GameplayReactiveProperty<DateTime>(other.NextCollectionDateTime.Value);
        }

        public override PlaceableModel Clone()
        {
            return new ProductionBuildingModel(this);
        }
    }
}