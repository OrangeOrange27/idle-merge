using System;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class StartProductionPopupPayload
    {
        public ProductionBuildingModel ProductionBuilding;
    }
}