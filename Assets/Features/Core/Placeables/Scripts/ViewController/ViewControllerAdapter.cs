using System;
using Features.Core.Placeables.Models;
using Features.Core.Placeables.Views;

namespace Features.Core.Placeables.ViewController
{
    public class ViewControllerAdapter<TModel> : IPlaceableViewControllerBase
        where TModel : PlaceableModel
    {
        private readonly IPlaceableViewController<TModel> _inner;

        public ViewControllerAdapter(IPlaceableViewController<TModel> inner)
        {
            _inner = inner;
        }

        public event Action<PlaceableModel> OnTap
        {
            add => _inner.OnTap += value;
            remove => _inner.OnTap -= value;
        }

        public void InitOnCreate(IPlaceableView view, PlaceableModel model)
            => _inner.InitOnCreate(view, (TModel)model);

        public void InitObserving() => _inner.InitObserving();

        public void Dispose() => _inner.Dispose();
    }
}