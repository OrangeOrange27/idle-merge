using System;
using Features.Core;
using Features.Core.GridSystem.Managers;
using Features.Core.MergeSystem.Scripts.Models;
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

            builder.RegisterViewLoader<PlaceableView, IPlaceableView, string>(key => key);
            builder.RegisterViewLoader<MergeableView, IPlaceableView, MergeableType>(mergeableType =>
            {
                return mergeableType switch
                {
                    MergeableType.Circle => "Circle",
                    MergeableType.Square => "Square",
                    _ => throw new ArgumentOutOfRangeException(nameof(mergeableType), mergeableType, null)
                };
            });
            
            builder.Register<PlaceableViewController>(Lifetime.Transient).AsImplementedInterfaces();
            builder.RegisterFactory<IPlaceableViewController>(resolver => resolver.Resolve<IPlaceableViewController>, Lifetime.Transient);
            builder.Register<PlaceablesVisualSystem>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlaceablesVisualProvider>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<GameplayController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}