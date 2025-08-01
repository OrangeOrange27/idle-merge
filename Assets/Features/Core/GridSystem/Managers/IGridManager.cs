﻿using System.Collections.ObjectModel;
using Features.Core.GridSystem.Tiles;
using UnityEngine;

namespace Features.Core.GridSystem.Managers
{
    public interface IGridManager
    {
        Grid Grid { get; }
        ReadOnlyDictionary<Vector3Int, GameAreaTile> ValidCells { get; }
        IGameAreaTile GetRandomFreeTile();
        IGameAreaTile GetTile(Vector3Int position);
        IGameAreaTile[] GetNeighbours(Vector3Int position);
        IGameAreaTile[] GetNeighbours(IGameAreaTile tile);
    }
}