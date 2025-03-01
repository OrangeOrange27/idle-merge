using Features.Core.GridSystem.Tiles;
using UnityEngine;

namespace Features.Core.GridSystem.Managers
{
    public interface IGridManager
    {
        IGameAreaTile GetRandomFreeTile();
        IGameAreaTile GetTile(Vector3Int position);
    }
}