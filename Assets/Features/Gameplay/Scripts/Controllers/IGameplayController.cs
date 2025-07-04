using System;
using Features.Core;
using Features.Core.Placeables.Models;

namespace Features.Gameplay.Scripts.Controllers
{
    public interface IGameplayController
    {
        GameContext GameContext { get; }
        
        event Action<ProductionBuildingModel> OnRequestCrafting;
        
        IDisposable Initialize(GameContext gameContext);
        void RegisterPlaceableClick(PlaceableModel placeableModel);
    }
}