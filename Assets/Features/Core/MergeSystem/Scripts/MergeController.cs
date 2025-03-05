using System;
using System.Collections.Generic;
using System.Linq;
using Features.Core.GridSystem.Managers;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Gameplay.View;
using UnityEngine;

namespace Features.Core.MergeSystem.Scripts
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
        
        public MergeController(PlaceablesFactoryResolver placeablesFactory,Func<IGameView> gameViewGetter)
        {
            _placeablesFactory = placeablesFactory;
            _gameViewGetter = gameViewGetter;
        }

        public bool TryMerge(GameContext gameContext, PlaceableModel placeable, IGameAreaTile targetTile)
        {
            if (placeable.CanMergeWith(targetTile.OccupyingObject) == false)
                return false;

            var connectedMergeables = GetAllConnectedMergeables(targetTile);
            if(!connectedMergeables.Contains(placeable))
                connectedMergeables.Add(placeable);
            
            if (connectedMergeables.Count < MinMergeableCount)
                return false;

            var resultObject = Merge(placeable, targetTile, connectedMergeables);
            if(resultObject!=null)
                gameContext.Placeables.Add(resultObject);
            
            if (connectedMergeables.Count >= MinBonusMergeableCount)
            {
                var bonusObject = CreateBonusObject(placeable, targetTile);
                if(bonusObject!=null)
                    gameContext.Placeables.Add(bonusObject);
            }

            return true;
        }

        private PlaceableModel Merge(PlaceableModel placeable, IGameAreaTile targetTile, List<PlaceableModel> connectedMergeables)
        {
            foreach (var mergeable in connectedMergeables)
                mergeable.Dispose();
            
            return CreateNextStageMergeable(placeable, targetTile);
        }

        //BUGGY TODO:REFACTOR
        private PlaceableModel CreateBonusObject(PlaceableModel placeable, IGameAreaTile targetTile)
        {
            var neighbourFreeTile = GridManager.GetNeighbours(targetTile)
                .FirstOrDefault(tile => tile.IsOccupied == false);
            var spawnTile = neighbourFreeTile ?? GridManager.GetRandomFreeTile();

            if(spawnTile == null)
                return null;
            
            return CreateNextStageMergeable(placeable, targetTile);
        }

        private PlaceableModel CreateNextStageMergeable(PlaceableModel placeable, IGameAreaTile spawnTile)
        {
            var resultObject = _placeablesFactory.Create(PlaceableType.MergeableObject);
            resultObject.MergeableType = placeable.MergeableType;
            resultObject.Stage.Value = placeable.Stage.Value + 1;
            resultObject.ParentTile.Value = spawnTile;

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