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
                    
                    MergeableType.Coin => "Coin",
                    MergeableType.Gem => "Gem",
                    MergeableType.Energy => "Energy",
                    MergeableType.BengalCat => "BengalCat",
                    MergeableType.MaineCoonCat => "MaineCoonCat",
                    MergeableType.RagdollCat => "RagdollCat",
                    MergeableType.SphynxCat => "SphynxCat",
                    MergeableType.BritishShorthairCat => "BritishShorthairCat",
                    MergeableType.ScottishFoldCat => "ScottishFoldCat",
                    MergeableType.OrientalCat => "OrientalCat",
                    MergeableType.RussianBlueCat => "RussianBlueCat",
                    MergeableType.TabbyCat => "TabbyCat",
                    MergeableType.SiameseCat => "SiameseCat",
                    MergeableType.PersianCat => "PersianCat",
                    _ => throw new ArgumentOutOfRangeException(nameof(mergeableType), mergeableType, null)
                };
            });
            builder.RegisterViewLoader<CollectibleView, IPlaceableView, CollectibleType>(collectibleType =>
            {
                return collectibleType switch
                {
                    CollectibleType.Fish => "FishCollectible",
                    CollectibleType.Herbs => "HerbsCollectible",
                    CollectibleType.Fur => "FurCollectible",
                    CollectibleType.Wood =>"WoodCollectible",
                    CollectibleType.Feather => "FeatherCollectible",
                    CollectibleType.Essence => "EssenceCollectible",
                    CollectibleType.Dust => "DustCollectible",
                    CollectibleType.ToyParts => "ToyPartsCollectible",
                    CollectibleType.Milk => "MilkCollectible",
                    CollectibleType.Crystal => "CrystalCollectible",
                    _ => throw new ArgumentOutOfRangeException(nameof(collectibleType), collectibleType, null)
                };
            });
            builder.RegisterViewLoader<ProductionObjectView, IPlaceableView, ProductionType>(productionType =>
            {
                return productionType switch
                {
                    ProductionType.Fish => "FishProducer",
                    ProductionType.Herbs => "HerbsProducer",
                    ProductionType.Fur => "FurProducer",
                    ProductionType.Wood => "WoodProducer",
                    ProductionType.Feather => "FeatherProducer",
                    ProductionType.Essence => "EssenceProducer",
                    ProductionType.Dust => "DustProducer",
                    ProductionType.ToyParts => "ToyPartsProducer",
                    ProductionType.Milk => "MilkProducer",
                    ProductionType.Crystal => "CrystalProducer",
                    _ => throw new ArgumentOutOfRangeException(nameof(productionType), productionType, null)
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