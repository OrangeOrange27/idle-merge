using Features.Core.Placeables.Models;
using Features.Core.Placeables.Views.Components;
using UnityEngine;

namespace Features.Core.Placeables.Views
{
    public class ProductionBuildingView : PlaceableView, IProductionBuildingView
    {
        [SerializeField] private ProductionBuildingTooltip _tooltip;
        
        public ProductionBuildingModel Model => _model as ProductionBuildingModel;

        public void ShowTooltip(ProductionBuildingTooltipType type)
        {
            _tooltip.SetType(type);
            _tooltip.Show();
        }
    }
}