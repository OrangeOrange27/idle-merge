using Features.Core.ProductionSystem.Models;
using UnityEngine;

namespace Features.Core.Placeables.Editor
{
    public class ProductionBuildingEditorModel : MonoBehaviour
    {
        public string Name;
        public ProductionRecipe[] AvailableRecipes;
        public ProductionRecipe SelectedRecipe;
    }
}