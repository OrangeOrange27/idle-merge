using System;
using System.Collections.Generic;
using Features.Core.Common.Models;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class ProductionRecipe
    { 
        public string RecipeName;
        public List<CollectibleItemUIModel> Ingredients = new();
        public List<MergeableItemUIModel> Outcome = new();
        public int CraftingTimeInSeconds { get; set; }
    }
}