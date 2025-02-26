using Features.Core.GridSystem.Tiles;

namespace Features.Core.GridSystem
{
    public interface IGridManager
    {
        IGameAreaTile GetRandomFreeTile();
    }
}