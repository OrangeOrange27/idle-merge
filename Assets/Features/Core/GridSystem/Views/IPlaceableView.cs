using System;
using Features.Core.GridSystem.Tiles;

namespace Features.Core
{
    public interface IPlaceableView
    {
        event Action OnTap;
        void SetModel(PlaceableModel model);
        void SetParentTile(IGameAreaTile tile);
    }
}