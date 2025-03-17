using System.Collections.Generic;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem
{
    public interface IProductionController
    {
        List<CollectibleModel> TryCollect(ProductionObjectModel productionObjectModel);
        List<PlaceableModel> Recycle(ProductionObjectModel productionObjectModel);
    }
}