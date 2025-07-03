using System;
using Features.Gameplay.Scripts.Models;

namespace Features.Gameplay.View
{
    public interface IGameUIView
    {
        event Action OnSupplyButtonClicked;

        void Initialize(GameUIDTO dto);
    }
}