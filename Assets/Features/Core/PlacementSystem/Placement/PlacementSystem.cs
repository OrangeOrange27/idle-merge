using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Features.Core.GridSystem.Managers;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Models;
using ObservableCollections;
using R3;
using UnityEngine;

namespace Features.Core.PlacementSystem
{
    public class PlacementSystem : IPlacementSystem
    {
        private GameContext _gameContext;
        private IGridManager _gridManager;

        public IGridManager GridManager => _gridManager;

        public event Action<PlacementRequestResult> OnPlacementAttempt;

        public IDisposable Initialize(GameContext gameContext, IGridManager gridManager)
        {
            _gameContext = gameContext;
            _gridManager = gridManager;

            var disposableBag = new DisposableBag();

            _gameContext.Placeables.CollectionChanged += OnCollectionChanged;
            disposableBag.Add(new Package.Disposables.Disposable(() =>
                _gameContext.Placeables.CollectionChanged -= OnCollectionChanged));
            foreach (var model in _gameContext.Placeables) SubscribeOnChanges(model);

            return disposableBag;

            void OnCollectionChanged(in NotifyCollectionChangedEventArgs<PlaceableModel> e)
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    if (e.IsSingleItem)
                    {
                        SubscribeOnChanges(e.NewItem);
                    }
                    else
                    {
                        foreach (var model in e.NewItems)
                        {
                            SubscribeOnChanges(model);
                        }
                    }
                }
            }

            void SubscribeOnChanges(PlaceableModel placeable)
            {
                ChangePlaceableTiles(placeable, Array.Empty<IGameAreaTile>(), placeable.OccupiedTiles);
                placeable.OccupiedTiles.CollectionChanged += ChangeTiles;
                disposableBag.Add(new Package.Disposables.Disposable(() =>
                    placeable.OccupiedTiles.CollectionChanged -= ChangeTiles));

                return;

                void ChangeTiles(in NotifyCollectionChangedEventArgs<IGameAreaTile> e) =>
                    ChangePlaceableTiles(placeable, e);
            }
        }

        public void PlaceOnRandomCell(PlaceableModel placeable)
        {
            TryPlaceOnCell(placeable, _gridManager.GetRandomFreeTile().Position);
        }

        public bool TryPlaceOnCell(PlaceableModel placeable, Vector3Int targetCellPosition)
        {
            var tilesToOccupy = new List<IGameAreaTile>();

            for (var i = 0; i < placeable.Size.x; i++)
            {
                for (var j = 0; j < placeable.Size.y; j++)
                {
                    var celPos = new Vector3Int(targetCellPosition.x + i, targetCellPosition.y + j,
                        targetCellPosition.z);

                    var tile = _gridManager.GetTile(celPos);
                    if (tile == null || tile.IsOccupied)
                    {
                        OnPlacementAttempt?.Invoke(new PlacementRequestResult()
                        {
                            IsSuccessful = false,
                            Placeable = placeable,
                            TargetCell = celPos
                        });
                        return false;
                    }

                    tilesToOccupy.Add(tile);
                }
            }

            ChangePlaceableTiles(placeable, placeable.OccupiedTiles, tilesToOccupy);

            OnPlacementAttempt?.Invoke(new PlacementRequestResult()
            {
                IsSuccessful = true,
                Placeable = placeable,
                TargetCell = targetCellPosition
            });
            return true;
        }

        private static void ChangePlaceableTiles(PlaceableModel placeable,
            in NotifyCollectionChangedEventArgs<IGameAreaTile> e)
        {
            ChangePlaceableTiles(placeable, e.OldItems.ToArray(), e.NewItems.ToArray());
        }

        private static void ChangePlaceableTiles(PlaceableModel placeable, IEnumerable<IGameAreaTile> oldTiles,
            IEnumerable<IGameAreaTile> newTiles)
        {
            foreach (var oldTile in oldTiles)
            {
                oldTile?.DeOccupy();
            }

            foreach (var newTile in newTiles)
            {
                newTile.Occupy(placeable);
            }
        }
    }
}