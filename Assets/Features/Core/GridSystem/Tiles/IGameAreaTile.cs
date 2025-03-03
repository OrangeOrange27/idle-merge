using UnityEngine;

namespace Features.Core.GridSystem.Tiles
{
    public interface IGameAreaTile
    {
        bool IsOccupied { get; }
        PlaceableModel OccupyingObject { get; }
        Vector3Int Position { get; }
        Transform Transform { get; }
        
        void Occupy(PlaceableModel gameAreaPlaceable);
        void DeOccupy();
    }
}