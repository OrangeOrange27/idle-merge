using Features.Core.Placeables.Models;

namespace Features.Core.SupplySystem
{
    public interface ISupplyProvider
    {
        PlaceableModel GetSupply();
    }
}