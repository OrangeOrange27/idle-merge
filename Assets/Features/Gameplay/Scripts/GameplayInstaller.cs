using System;
using Features.Core.GridSystem.Managers;
using Features.Gameplay.Scripts.Controllers;
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

            builder.Register<IGridManager, GridManager>(Lifetime.Singleton);
            builder.Register(resolver => new Lazy<IGridManager>(resolver.Resolve<IGridManager>), Lifetime.Singleton);

            builder.Register<GameplayController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}