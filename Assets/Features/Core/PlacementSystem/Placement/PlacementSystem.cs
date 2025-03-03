using System;
using System.Collections.Specialized;
using Features.Core.GridSystem.Tiles;
using ObservableCollections;
using R3;

namespace Features.Core.PlacementSystem
{
    public class PlacementSystem : IPlacementSystem
    {
        private GameContext _gameContext;
        
        public IDisposable Initialize(GameContext gameContext)
        {
            _gameContext = gameContext;

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
        
        private static void ChangePlaceableTile(PlaceableModel placeable, IGameAreaTile oldTile, IGameAreaTile newTile)
        {
            oldTile?.DeOccupy();
            newTile.Occupy(placeable);
        }
    }
}