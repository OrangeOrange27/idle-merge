using Features.Core;

namespace Features.Gameplay.Scripts.Controllers
{
    public interface IGameplayController
    {
        GameContext GameContext { get; }
        
        void Initialize(GameContext gameContext);
        void RegisterPlaceableClick(PlaceableModel placeableModel);
    }
}