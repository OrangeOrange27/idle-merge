using System;
using Features.Core.Placeables.Models;

namespace Features.Core.Placeables.Views
{
    public interface IPlaceableViewController : IDisposable
    {
        event Action<PlaceableModel> OnTap;
        void InitOnCreate(GameContext context, IPlaceableView view, PlaceableModel model);
        void InitObserving();
    }
}