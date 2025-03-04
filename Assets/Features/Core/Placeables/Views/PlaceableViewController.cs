using System;
using Features.Core.Placeables.Models;
using R3;

namespace Features.Core.Placeables.Views
{
    public class PlaceableViewController : IPlaceableViewController
    {
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
        }

        public void InitObserving()
        {
            _view.OnTap += OnViewTap;

            _disposable = Disposable.Combine(
                _model.ParentTile.Subscribe(tile => _view.SetParentTile(tile)),
                _model.Position.Subscribe(position => _view.Move(position)),
                _model.Stage.Subscribe(stage => _view.SetStage(stage))
                );
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