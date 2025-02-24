using Features.Gameplay.View;
using Package.AssetProvider.ViewLoader.VContainer;
using VContainer;

namespace Features.Gameplay.Scripts
{
    public static class GameplayInstaller
    {
        public static void RegisterGameplay(this IContainerBuilder builder)
        {
            builder.RegisterSharedViewLoader<GameView, IGameView>("GameView");
        }
    }
}