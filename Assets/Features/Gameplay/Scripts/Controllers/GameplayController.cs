using Features.Core;
using Features.Core.PlacementSystem;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;

namespace Features.Gameplay.Scripts.Controllers
{
    public class GameplayController : IGameplayController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameplayController>();

        private readonly IPlacementController _placementController;

        private GameContext _gameContext;

        public GameContext GameContext => _gameContext;
        
        public GameplayController(IPlacementController placementController)
        {
            _placementController = placementController;
        }
        
        public void Initialize(GameContext gameContext)
        {
            _gameContext = gameContext;
        }
        
        public void RegisterPlaceableClick(PlaceableModel placeableModel)
        {
            Logger.LogInformation($"Placeable {placeableModel} clicked");
            _placementController.SelectPlaceable(placeableModel);
        }
    }
}