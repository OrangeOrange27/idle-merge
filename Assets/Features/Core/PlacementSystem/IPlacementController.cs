namespace Features.Core.PlacementSystem
{
    public interface IPlacementController
    {
        void SelectPlaceable(PlaceableModel placeable);
        void DeSelectPlaceable();
    }
}