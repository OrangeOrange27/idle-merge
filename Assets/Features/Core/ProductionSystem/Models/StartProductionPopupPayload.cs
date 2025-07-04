using System;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class StartProductionPopupPayload
    {
        public ProductionRecipe[] AvailableRecipes;
    }
}