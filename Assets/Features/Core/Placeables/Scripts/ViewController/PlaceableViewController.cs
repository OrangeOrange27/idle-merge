using System;
using Features.Core.Placeables.Models;
using R3;

namespace Features.Core.Placeables.Views
{
    public class PlaceableViewController<TModel> : IPlaceableViewController<TModel>
        where TModel : PlaceableModel
    {
        private TModel _model;
        private IPlaceableView _view;
        private IDisposable _disposable;

        public event Action<TModel> OnTap;
        private event Action<PlaceableModel> _baseOnTap;

        public void InitOnCreate(IPlaceableView view, TModel model)
        {
            _view = view;
            _model = model;
            _view.SetModel(_model);
        }

        public void InitObserving()
        {
            _view.OnTap += OnViewTap;

            _disposable = Disposable.Combine(
                _model.ParentTile.Subscribe(tile => _view.SetParentTile(tile)),
                _model.Position.Subscribe(position => _view.Move(position))
            );

            if (_model is MergeableModel mergeableModel)
            {
                _disposable = Disposable.Combine(
                    _disposable,
                    mergeableModel.Stage.Subscribe(stage => _view.SetStage(stage)));
            }
        }
        
        void IPlaceableViewControllerBase.InitOnCreate(IPlaceableView view, PlaceableModel model)
        {
            InitOnCreate(view, (TModel)model);
        }
        
        event Action<PlaceableModel> IPlaceableViewControllerBase.OnTap
        {
            add => _baseOnTap += value;
            remove => _baseOnTap -= value;
        }

        public void Dispose()
        {
            _view.OnTap -= OnViewTap;
            _disposable?.Dispose();
        }

        private void OnViewTap()
        {
            OnTap?.Invoke(_model);
            _baseOnTap?.Invoke(_model);
        }
    }
}