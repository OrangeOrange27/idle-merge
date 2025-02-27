using System;

namespace Features.Core
{
    public interface IPlaceableView
    {
        event Action OnTap;
        void SetModel(PlaceableModel model);
    }
}