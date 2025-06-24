using Common.UI;

namespace Features.Core.Placeables.Views.Components
{
    public enum ProductionBuildingTooltipType
    {
        ReadyToCollect,
        CanStartProduction,
    }
    public class ProductionBuildingTooltip : Tooltip
    {
        public ProductionBuildingTooltipType Type { get; private set; }
        
        public void SetType(ProductionBuildingTooltipType type)
        {
            Type = type;
        }
    }
}