using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem
{
    public interface IProductionController
    {
        CollectibleModel TryCollect(ProductionObjectModel productionObjectModel);
    }
}