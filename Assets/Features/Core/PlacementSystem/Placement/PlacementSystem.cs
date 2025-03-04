using System;
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
                var disposable = Disposable.Combine(
                    placeable.ParentTile.Subscribe(tile =>
                        ChangePlaceableTile(placeable, placeable.ParentTile.PreviousValue, tile))
                );
                disposableBag.Add(disposable);
            }
        }

        public event Action<PlacementRequestResult> OnPlacementAttempt; 
        
        public bool TryPlaceOnCell(PlaceableModel placeable, Vector3Int cellPosition)
        {
            var tile = _gridManager.GetTile(cellPosition);
            if (tile == null || tile.IsOccupied)
            {
                OnPlacementAttempt?.Invoke(new PlacementRequestResult()
                {
                    IsSuccessful = false,
                    Placeable = placeable,
                    TargetCell = cellPosition
                });
                return false;
            }

            placeable.ParentTile.Value = tile;
            OnPlacementAttempt?.Invoke(new PlacementRequestResult()
            {
                IsSuccessful = true,
                Placeable = placeable,
                TargetCell = cellPosition
            });
            return true;
        }
        
        private static void ChangePlaceableTile(PlaceableModel placeable, IGameAreaTile oldTile, IGameAreaTile newTile)
        {
            oldTile?.DeOccupy();
            newTile.Occupy(placeable);
        }
    }
}