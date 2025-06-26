using System;
using System.Collections.Generic;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class ProductionRecipe
    { 
        public class Component
        {
            public CollectibleType CollectibleType { get; set; }
            public int Amout { get; set; }
        }

        public string RecipeName;
        public List<Component> InComponents = new();
        public List<MergeableType> Outcome = new();
        public int CraftingTimeInSeconds { get; set; }
    }
}