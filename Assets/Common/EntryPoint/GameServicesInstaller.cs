using System;
using Common.ApplicationLifecycleNotifier;
using Common.Audio;
using Common.Audio.Implementation;
using Common.Audio.Infrastructure;
using Common.DataProvider.Infrastructure;
using Common.DataProvider.Storage.Implementation.File;
using Common.DataProvider.Storage.Infrastructure;
using Common.DeviceInfo;
using Common.Encoding.Implementation;
using Common.Encoding.Infrastructure;
using Common.EntryPoint.Initialize;
using Common.GlobalServiceLocator;
using Common.JsonConverters;
using Common.PlayerData;
using Common.Serialization;
using Common.TimeService;
using Features.Core;
using Features.Core.MergeSystem;
using Features.Core.Placeables;
using Features.Core.Placeables.Factories;
using Features.Core.PlacementSystem;
using Features.Core.SupplySystem;
using Features.Core.SupplySystem.Providers;
using Features.Gameplay.Scripts;
using Features.SplashScreen;
using Newtonsoft.Json;
using Package.AssetProvider.Implementation;
using Package.AssetProvider.Infrastructure;
using Package.ControllersTree.VContainer;
using Package.Logger.Abstraction;
using Package.Pooling.Installers;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.EntryPoint
{
    public class GameServicesInstaller : LifetimeScope
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameServicesInstaller>();

        [SerializeField] private LifetimeScope[] _compositeScopes;
        [SerializeField] private SplashSceneView _splashSceneView;
        [SerializeField] private SupplyWeightsConfigProvider supplyWeightsConfigProvider;

        public static JsonSerializerSettings JsonSettings = new()
        {
            Formatting = Formatting.Indented,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            Converters = new JsonConverter[]
            {
                Vector2Converter.Singleton,
                Vector3Converter.Singleton,
                Vector2IntConverter.Singleton,
                Vector3IntConverter.Singleton,
            }
        };

        public Action<IContainerBuilder> OutsideBuilder;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterSerialization();
            RegisterDataProvider(builder);
            builder.RegisterFactory<Type, ILogger>(_ => (type) => LogManager.GetLogger(type.Name), Lifetime.Singleton);

            builder.Register<IAssetProviderAnalyticsCallbacks, EmptyAssetProviderAnalyticsCallbacks>(
                Lifetime.Singleton);
            builder.RegisterDefaultPoolStrategy();
            builder.RegisterGameObjectPrefabsPool();
            builder.RegisterTimeService();

            builder.RegisterApplicationLifecycleNotifier();
            builder.RegisterDeviceInfoRegistration();

            builder.RegisterControllersTreePackage();
            builder.RegisterController<RootController>();
            builder.RegisterController<InitializeGameAfterAuthController>();
            builder.RegisterController<InitializeGameBeforeAuthController>();

            builder.Register<AudioManagerInitController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<IAudioManager, AudioManager>(Lifetime.Singleton);

            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Transient);
            builder.RegisterFactory<IAssetProvider>(resolver => resolver.Resolve<IAssetProvider>, Lifetime.Transient);

            builder.RegisterInstance(JsonSettings);
            builder.Register<PlayerDataService>(Lifetime.Singleton).AsImplementedInterfaces();

            builder.RegisterSplashScreen(_splashSceneView);
            RegisterSystems(builder);
            builder.RegisterGameplay();

            OutsideBuilder?.Invoke(builder);
            builder.RegisterBuildCallback(GlobalServices.Initialize);
        }

        private void RegisterSystems(IContainerBuilder builder)
        {
            builder.RegisterPlaceables();
            builder.RegisterMergeSystem(supplyWeightsConfigProvider);
            builder.RegisterSupplySystem();
            builder.RegisterPlacementSystem();
        }

        private void RegisterDataProvider(IContainerBuilder builder)
        {
            builder.Register<IDataProvider, DataProviderBase>(Lifetime.Singleton);
            builder.Register<IDataStorage, PlayerDataFileDataStorage>(Lifetime.Singleton);
#if UNITY_EDITOR
            builder.Register<IEncoder, GenericEncoder>(Lifetime.Singleton);
#else
            builder.Register<IEncoder, CryptEncoder>(Lifetime.Singleton);
#endif
        }
    }
}