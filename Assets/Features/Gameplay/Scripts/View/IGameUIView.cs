using System;

namespace Features.Gameplay.View
{
    public interface IGameUIView
    {
        event Action OnSupplyButtonClicked;
    }
}