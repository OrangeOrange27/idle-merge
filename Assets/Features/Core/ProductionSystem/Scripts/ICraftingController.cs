using System.Collections.Generic;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;

namespace Features.Core.ProductionSystem
{
    public interface ICraftingController
    {
        void StartCrafting(ProductionBuildingModel productionBuildingModel, ProductionRecipe recipe);
        bool CanStartCrafting(ProductionBuildingModel productionBuildingModel, ProductionRecipe recipe);
        List<MergeableModel> TryCollect(ProductionBuildingModel productionBuildingModel);
    }
}