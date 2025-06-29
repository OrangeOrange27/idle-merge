using System;
using System.Linq;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Models;
using Microsoft.Extensions.Logging;
using ObservableCollections;
using Package.Logger.Abstraction;
using R3;

namespace Features.Core.Placeables.Views
{
    public class PlaceableViewController : IPlaceableViewController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<PlaceableViewController>();
        
        private PlaceableModel _model;
        private IPlaceableView _view;
        private GameContext _context;
        private IDisposable _disposable;

        public event Action<PlaceableModel> OnTap;

        public void InitOnCreate(GameContext context, IPlaceableView view, PlaceableModel model)
        {
            _context = context;
            _view = view;
            _model = model;
            _view.SetModel(_model);
            
            UpdateParentTile(_model.OccupiedTiles.First());
        }

        public void InitObserving()
        {
            _view.OnTap += OnViewTap;
            _model.OccupiedTiles.CollectionChanged += OnParentTileChange;
            
            _disposable = Disposable.Combine(
                new Package.Disposables.Disposable(() =>
                    _model.OccupiedTiles.CollectionChanged -= OnParentTileChange),
                
                _model.Position.Subscribe(position => _view.Move(position))
            );

            if (_model is MergeableModel mergeableModel)
            {
                _disposable = Disposable.Combine(
                    _disposable,
                    mergeableModel.Stage.Subscribe(stage => _view.SetStage(stage)));
            }

            return;

            void OnParentTileChange(in NotifyCollectionChangedEventArgs<IGameAreaTile> e)
            {
                if (e.NewItems.IsEmpty)
                {
                    Logger.LogDebug($"Parent tile for {_model.Position} tile has changed to null");
                    return;
                }

                UpdateParentTile(e.NewItems[0]);
            }
        }

        private void UpdateParentTile(in IGameAreaTile tile)
        {
            _model.Position.Value = tile.Position + PlaceablesConstants.PlaceableOffset;
        }

        public void Dispose()
        {
            _view.OnTap -= OnViewTap;

            _disposable?.Dispose();
        }

        private void OnViewTap()
        {
            OnTap?.Invoke(_model);
        }
    }
}