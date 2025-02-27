using Features.Core.GridSystem.Tiles;
using UnityEngine;

namespace Features.Core.GridSystem
{
    public interface IGridManager
    {
        IGameAreaTile GetRandomFreeTile();
        IGameAreaTile GetTile(Vector3Int position);
    }
}