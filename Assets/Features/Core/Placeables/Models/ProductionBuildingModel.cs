using System;
using Features.Core.ProductionSystem.Models;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class ProductionBuildingModel : PlaceableModel
    {
        public string Name;
        public ProductionRecipe[] AvailableRecipes;
        public ProductionRecipe SelectedRecipe;
        public GameplayReactiveProperty<DateTime> NextCollectionDateTime = new();
        public GameplayReactiveProperty<bool> IsCrafting = new();
        
        public ProductionBuildingModel()
        {
        }

        protected ProductionBuildingModel(ProductionBuildingModel other) : base(other)
        {
            Name = other.Name;
            AvailableRecipes = other.AvailableRecipes;
            SelectedRecipe = other.SelectedRecipe;
            NextCollectionDateTime = new GameplayReactiveProperty<DateTime>(other.NextCollectionDateTime.Value);
            IsCrafting = new GameplayReactiveProperty<bool>(other.IsCrafting.Value);
        }

        public override PlaceableModel Clone()
        {
            return new ProductionBuildingModel(this);
        }
    }
}