using System;

namespace Features.Core.PlacementSystem
{
    public interface ISelectionController
    {
        event Action<PlaceableModel> OnSelect;
        event Action<PlaceableModel> OnDeSelect;
        
        void SelectPlaceable(PlaceableModel placeable);
    }
}