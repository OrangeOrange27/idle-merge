using System;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.PlacementSystem
{
    public interface IPlacementSystem
    {
        event Action<PlacementRequestResult> OnPlacementAttempt; 
        
        IDisposable Initialize(GameContext gameContext, IGridManager gridManager);
        bool TryPlaceOnCell(PlaceableModel placeable, Vector3Int cellPosition);
        void PlaceOnRandomCell(PlaceableModel placeable);
    }
}