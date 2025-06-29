using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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

                void ChangeTiles(in NotifyCollectionChangedEventArgs<IGameAreaTile> e)
                {
                    if (e.Action is NotifyCollectionChangedAction.Remove)
                    {
                        if (e.IsSingleItem)
                        {
                            e.OldItem?.DeOccupy();
                        }
                        else
                        {
                            foreach (var tile in e.OldItems)
                            {
                                tile?.DeOccupy();
                            }
                        }
                    }
                    else
                    {
                        ChangePlaceableTiles(placeable, e);
                    }
                }
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

            // This will trigger a NotifyCollectionChangedAction.Remove event for each item
            // instead of NotifyCollectionChangedAction.Reset
            foreach (var item in placeable.OccupiedTiles.ToList())
            {
                placeable.OccupiedTiles.Remove(item);
            }

            placeable.OccupiedTiles.AddRange(tilesToOccupy);

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
            var oldItems = new List<IGameAreaTile>();
            var newItems = new List<IGameAreaTile>();

            if (e.IsSingleItem)
            {
                oldItems.Add(e.OldItem);
                newItems.Add(e.NewItem);
            }
            else
            {
                oldItems.AddRange(e.OldItems.ToArray());
                newItems.AddRange(e.NewItems.ToArray());
            }

            ChangePlaceableTiles(placeable, oldItems, newItems);
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