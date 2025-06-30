using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;
using UnityEngine;

namespace Features.Core.Placeables.Editor
{
    public class ProductionBuildingEditorModel : MonoBehaviour
    {
        public string Name;
        public ProductionRecipe[] AvailableRecipes;
        public Vector2 Size;

        public ProductionBuildingModel GetModel()
        {
            return new ProductionBuildingModel()
            {
                ObjectType = PlaceableType.ProductionBuilding,
                Size = Size,
                
                Name = Name,
                AvailableRecipes = AvailableRecipes,
            };
        }
    }
}