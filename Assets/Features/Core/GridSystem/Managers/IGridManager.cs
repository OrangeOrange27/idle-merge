using Features.Core.GridSystem.Tiles;
using UnityEngine;

namespace Features.Core.GridSystem.Managers
{
    public interface IGridManager
    {
        Grid Grid { get; }
        IGameAreaTile GetRandomFreeTile();
        IGameAreaTile GetTile(Vector3Int position);
    }
}