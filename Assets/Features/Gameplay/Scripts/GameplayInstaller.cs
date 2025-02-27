using Features.Core;
using Features.Core.Grid.Managers;
using Features.Gameplay.States;
using Features.Gameplay.View;
using Package.AssetProvider.ViewLoader.VContainer;
using Package.ControllersTree.VContainer;
using VContainer;

namespace Features.Gameplay.Scripts
{
    public static class GameplayInstaller
    {
        public static void RegisterGameplay(this IContainerBuilder builder)
        {
            builder.RegisterController<RootGameplayState>();
            builder.RegisterSharedViewLoader<GameView, IGameView>("GameView");
            
            builder.Register<PlaceableViewController>(Lifetime.Transient).AsImplementedInterfaces();
            builder.RegisterFactory<IPlaceableViewController>(resolver => resolver.Resolve<IPlaceableViewController>, Lifetime.Transient);
            builder.Register<PlaceablesVisualSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlaceablesVisualProvider>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}