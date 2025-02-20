using UnityEngine;

namespace Core.GridSystem.Tiles
{
    public interface IGameAreaTile
    {
        bool IsOccupied { get; }
        IGameAreaPlaceable OccupyingObject { get; }
        Vector3Int Position { get; }
        
        void Occupy(IGameAreaPlaceable gameAreaPlaceable);
    }
}