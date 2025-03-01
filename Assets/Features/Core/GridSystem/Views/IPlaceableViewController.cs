using System;

namespace Features.Core
{
    public interface IPlaceableViewController : IDisposable
    {
        event Action<PlaceableModel> OnTap;
        void InitOnCreate(GameContext context, IPlaceableView view, PlaceableModel model);
        void InitObserving();
    }
}