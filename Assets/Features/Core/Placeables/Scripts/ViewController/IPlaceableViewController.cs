using System;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Views
{
    public interface IPlaceableViewControllerBase : IDisposable
    {
        event Action<PlaceableModel> OnTap;
        void InitOnCreate(IPlaceableView view, PlaceableModel model);
        void InitObserving();
    }
    
    public interface IPlaceableViewController<TModel> : IPlaceableViewControllerBase
        where TModel : PlaceableModel
    {
        new event Action<TModel> OnTap;
        void InitOnCreate(IPlaceableView view, TModel model);
    }
}