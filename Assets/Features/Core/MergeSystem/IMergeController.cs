using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Models;

namespace Features.Core.MergeSystem
{
    public interface IMergeController
    {
        bool TryMerge(GameContext gameContext, PlaceableModel placeable, IGameAreaTile targetTile);
    }
}