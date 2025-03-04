using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Factories
{
    public class MergeablesFactory : IPlaceablesFactory
    {
        public PlaceableModel Create()
        {
            return new PlaceableModel() { ObjectType = PlaceableType.MergeableObject };
        }
    }
}