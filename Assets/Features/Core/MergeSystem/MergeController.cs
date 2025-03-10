using System;
using System.Collections.Generic;
using Common.Utils.Extensions;
using Features.Core.GridSystem.Managers;
using Features.Core.GridSystem.Tiles;
using Features.Core.MergeSystem.Providers;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Gameplay.View;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.MergeSystem
{
    public class MergeController : IMergeController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<MergeController>();

        private const int MinMergeableCount = 3;
        private const int MinBonusMergeableCount = 5;

        private readonly PlaceablesFactoryResolver _placeablesFactory;
        private readonly IMergeProvider _mergeProvider;
        private readonly Func<IGameView> _gameViewGetter;

        private IGameView _gameView;

        private IGameView GameView => _gameView ??= _gameViewGetter.Invoke();
        private IGridManager GridManager => GameView.GameAreaView.GridManager;

        public MergeController(PlaceablesFactoryResolver placeablesFactory, Func<IGameView> gameViewGetter, IMergeProvider mergeProvider)
        {
            _placeablesFactory = placeablesFactory;
            _gameViewGetter = gameViewGetter;
            _mergeProvider = mergeProvider;
        }

        public bool TryMerge(GameContext gameContext, MergeableModel placeable, IGameAreaTile targetTile)
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

        private List<PlaceableModel> Merge(MergeableModel placeable, IGameAreaTile targetTile,
            List<MergeableModel> connectedMergeables)
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

            return HandleMergeResult(totalNextTierCount, sameTierCount, placeable, targetTile);
        }

        private List<PlaceableModel> HandleMergeResult(int nextTierCount, int sameTierCount, MergeableModel originalObject, IGameAreaTile targetTile)
        {
            var result = new List<PlaceableModel>();
            var nextTierObject = _mergeProvider.Get(originalObject.MergeableType, originalObject.Stage.Value);

            if (nextTierObject == null)
            {
                Logger.ZLogError("Could not find merge result object");
                return result;
            }

            for (var i = 0; i < nextTierCount; i++)
                result.Add(CreateAndPositionPlaceable(nextTierObject, GetFreeTile(targetTile)));
            for (var i = 0; i < sameTierCount; i++)
                result.Add(CreateAndPositionPlaceable(originalObject, GetFreeTile(targetTile)));

            return result;
        }

        private PlaceableModel CreateAndPositionPlaceable(PlaceableModel originalObject, IGameAreaTile targetTile)
        {
            var tile = GetFreeTile(targetTile);
            if (tile == null)
            {
                Logger.ZLogError("Could not find free tile for merge result object");
                return null;
            }
                
            var model = originalObject.Clone();
            model.ParentTile.Value = tile;
            tile.Occupy(model);

            return model;
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

        private List<MergeableModel> GetAllConnectedMergeables(IGameAreaTile startTile)
        {
            var connectedMergeables = new List<MergeableModel>();
            var tilesToCheck = new Queue<IGameAreaTile>();
            var visitedTiles = new HashSet<Vector3Int>();

            tilesToCheck.Enqueue(startTile);
            visitedTiles.Add(startTile.Position);
            connectedMergeables.Add(startTile.OccupyingObject as MergeableModel);

            while (tilesToCheck.Count > 0)
            {
                var currentTile = tilesToCheck.Dequeue();

                var neighbours = GridManager.GetNeighbours(currentTile);

                foreach (var neighbourTile in neighbours)
                {
                    if (visitedTiles.Contains(neighbourTile.Position) || !neighbourTile.IsOccupied)
                        continue;

                    if ((neighbourTile.OccupyingObject as MergeableModel).CanMergeWith(currentTile.OccupyingObject))
                    {
                        connectedMergeables.Add(neighbourTile.OccupyingObject as MergeableModel);
                        visitedTiles.Add(neighbourTile.Position);
                        tilesToCheck.Enqueue(neighbourTile);
                    }
                }
            }

            return connectedMergeables;
        }
    }
}