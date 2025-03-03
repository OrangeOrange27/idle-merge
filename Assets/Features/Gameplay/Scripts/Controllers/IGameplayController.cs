using System;
using Features.Core;

namespace Features.Gameplay.Scripts.Controllers
{
    public interface IGameplayController
    {
        GameContext GameContext { get; }
        
        IDisposable Initialize(GameContext gameContext);
        void RegisterPlaceableClick(PlaceableModel placeableModel);
    }
}