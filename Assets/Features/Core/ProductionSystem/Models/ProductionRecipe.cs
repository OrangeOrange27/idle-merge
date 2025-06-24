using System.Collections.Generic;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    public class ProductionRecipe
    {
        public class Component
        {
            public ProductionType ProductionType { get; set; }
            public int Amout { get; set; }
        }

        public List<Component> InComponents = new();
        public List<Component> OutComponents = new();
        public int CraftingTimeInSeconds { get; set; }
    }
}