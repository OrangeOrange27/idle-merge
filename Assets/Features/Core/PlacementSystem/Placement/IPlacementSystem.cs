using System;

namespace Features.Core.PlacementSystem
{
    public interface IPlacementSystem
    {
        IDisposable Initialize(GameContext gameContext);
    }
}