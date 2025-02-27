using System;

namespace Features.Core.GridSystem.Tiles
{
    public interface IGameAreaPlaceable
    {
        event Action OnTap;

        IGameAreaTile ParentTile { get; }

        void SetParentTile(IGameAreaTile gameAreaTile);
    }
}