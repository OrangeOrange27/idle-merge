using Features.Core.Placeables.Models;

namespace Features.Core.SupplySystem.Providers
{
    public interface ISupplyProvider
    {
        PlaceableModel GetSupply();
    }
}