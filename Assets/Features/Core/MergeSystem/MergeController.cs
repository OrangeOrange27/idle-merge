using System;
using System.Collections.Generic;
using Common.Utils.Extensions;
using Features.Core.GridSystem.Managers;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Gameplay.View;
using UnityEngine;

namespace Features.Core.MergeSystem
{
    public class MergeController : IMergeController
    {
        private const int MinMergeableCount = 3;
        private const int MinBonusMergeableCount = 5;

        private readonly PlaceablesFactoryResolver _placeablesFactory;
        private readonly Func<IGameView> _gameViewGetter;

        private IGameView _gameView;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;

        public MergeController(PlaceablesFactoryResolver placeablesFactory, Func<IGameView> gameViewGetter)
        {
            _placeablesFactory = placeablesFactory;
            _gameViewGetter = gameViewGetter;
        }

        public bool TryMerge(GameContext gameContext, PlaceableModel placeable, IGameAreaTile targetTile)
        {
            if (placeable.CanMergeWith(targetTile.OccupyingObject) == false)
                return false;

            var connectedMergeables = GetAllConnectedMergeables(targetTile);
            if (!connectedMergeables.Contains(placeable))
                connectedMergeables.Add(placeable);

            if (connectedMergeables.Count < MinMergeableCount)
                return false;

            var mergeResult = Merge(placeable, targetTile, connectedMergeables);
            if (!mergeResult.IsNullOrEmpty())
                gameContext.Placeables.AddRange(mergeResult);

            return true;
        }

        private List<PlaceableModel> Merge(PlaceableModel placeable, IGameAreaTile targetTile,
            List<PlaceableModel> connectedMergeables)
        {
            var baseNextTierCount = connectedMergeables.Count / MinMergeableCount; // Standard merging (3 = 1 next-tier)
            var mergeBonusCount =
                connectedMergeables.Count % MinBonusMergeableCount == 0 // Extra next-tier items for every full set of 5
                    ? connectedMergeables.Count / MinBonusMergeableCount
                    : 0;
            var totalNextTierCount = baseNextTierCount + mergeBonusCount;
            var sameTierCount =
                mergeBonusCount > 0
                    ? 0
                    : connectedMergeables.Count % MinMergeableCount; // Remaining items at the same tier

            foreach (var mergeable in connectedMergeables)
                mergeable.Dispose();

            var result = new List<PlaceableModel>();
            for (int i = 0; i < totalNextTierCount; i++)
                result.Add(CreateNextStageMergeable(placeable, GetFreeTile(targetTile)));
            for (int i = 0; i < sameTierCount; i++)
                result.Add(CreateMergeable(placeable, GetFreeTile(targetTile)));

            return result;
        }

        private IGameAreaTile GetFreeTile(IGameAreaTile targetTile)
        {
            if (targetTile.IsOccupied == false)
                return targetTile;

            foreach (var tile in GridManager.GetNeighbours(targetTile))
            {
                if (tile.IsOccupied == false)
                    return tile;
            }

            return GridManager.GetRandomFreeTile();
        }

        private PlaceableModel CreateNextStageMergeable(PlaceableModel referenceModel, IGameAreaTile spawnTile)
        {
            var resultObject = CreateMergeable(referenceModel, spawnTile);
            resultObject.Stage.Value++;
            return resultObject;
        }

        private PlaceableModel CreateMergeable(PlaceableModel referenceModel, IGameAreaTile spawnTile)
        {
            var resultObject = _placeablesFactory.Create(PlaceableType.MergeableObject, referenceModel.MergeableType);
            resultObject.Stage.Value = referenceModel.Stage.Value;
            resultObject.ParentTile.Value = spawnTile;
            spawnTile.Occupy(resultObject);

            return resultObject;
        }

        private List<PlaceableModel> GetAllConnectedMergeables(IGameAreaTile startTile)
        {
            var connectedMergeables = new List<PlaceableModel>();
            var tilesToCheck = new Queue<IGameAreaTile>();
            var visitedTiles = new HashSet<Vector3Int>();

            tilesToCheck.Enqueue(startTile);
            visitedTiles.Add(startTile.Position);
            connectedMergeables.Add(startTile.OccupyingObject);

            while (tilesToCheck.Count > 0)
            {
                var currentTile = tilesToCheck.Dequeue();

                var neighbours = GridManager.GetNeighbours(currentTile);

                foreach (var neighbourTile in neighbours)
                {
                    if (visitedTiles.Contains(neighbourTile.Position) || !neighbourTile.IsOccupied)
                        continue;

                    if (neighbourTile.OccupyingObject.CanMergeWith(currentTile.OccupyingObject))
                    {
                        connectedMergeables.Add(neighbourTile.OccupyingObject);
                        visitedTiles.Add(neighbourTile.Position);
                        tilesToCheck.Enqueue(neighbourTile);
                    }
                }
            }

            return connectedMergeables;
        }
    }
}