using System;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.Placeables.Views;
using Features.Core.Placeables.VisualSystem;
using Package.AssetProvider.ViewLoader.VContainer;
using VContainer;

namespace Features.Core.Placeables
{
    public static class PlaceablesRegistration
    {
        public static void RegisterPlaceables(this IContainerBuilder builder)
        {
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
            
            builder.Register<IPlaceablesFactory, MergeablesFactory>(Lifetime.Singleton)
                .WithParameter("type", PlaceableType.MergeableObject);
            builder.Register<IPlaceablesFactory, CollectiblesFactory>(Lifetime.Singleton)
                .WithParameter("type", PlaceableType.CollectibleObject);            
            builder.Register<IPlaceablesFactory, ProductionObjectsFactory>(Lifetime.Singleton)
                .WithParameter("type", PlaceableType.ProductionEntity);
            
            builder.Register<PlaceablesFactoryResolver>(Lifetime.Singleton);
        }
    }
}