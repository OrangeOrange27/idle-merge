using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public interface IPlaceablesFactory
    {
        PlaceableModel Create();
    }
}