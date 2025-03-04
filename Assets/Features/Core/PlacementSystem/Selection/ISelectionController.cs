using System;
using Features.Core.Placeables.Models;

namespace Features.Core.PlacementSystem
{
    public interface ISelectionController
    {
        event Action<PlaceableModel> OnSelect;
        event Action<PlaceableModel> OnDeselect;
        
        void SelectPlaceable(PlaceableModel placeable);
    }
}