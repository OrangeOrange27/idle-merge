using System;
using Features.Core;
using Features.Core.PlacementSystem;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using R3;

namespace Features.Gameplay.Scripts.Controllers
{
    public class GameplayController : IGameplayController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameplayController>();

        private readonly ISelectionController _selectionController;

        private GameContext _gameContext;

        public GameContext GameContext => _gameContext;
        
        public GameplayController(ISelectionController selectionController)
        {
            _selectionController = selectionController;
        }

        public IDisposable Initialize(GameContext gameContext)
        {
            _gameContext = gameContext;
            return Disposable.Empty;
        }

        public void RegisterPlaceableClick(PlaceableModel placeableModel)
        {
            Logger.LogInformation($"Placeable {placeableModel} clicked");
            _selectionController.SelectPlaceable(placeableModel);
        }
    }
}