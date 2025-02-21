using System;
using Common.Audio;
using Common.Audio.Implementation;
using Common.Audio.Infrastructure;
using Common.GlobalServiceLocator;
using Common.JsonConverters;
using Newtonsoft.Json;
using Package.AssetProvider.Implementation;
using Package.AssetProvider.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common
{
    public class GameServicesInstaller : LifetimeScope
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameServicesInstaller>();

        [SerializeField] private LifetimeScope[] _compositeScopes;
        
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
            builder.RegisterFactory<Type, ILogger>(_ => (type) => LogManager.GetLogger(type.Name), Lifetime.Singleton);

            builder.Register<AudioManagerInitController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<IAudioManager, AudioManager>(Lifetime.Singleton);
            
            builder.Register<IAssetProvider, AddressablesAssetProvider>(Lifetime.Transient);
            builder.RegisterFactory<IAssetProvider>(resolver => resolver.Resolve<IAssetProvider>, Lifetime.Transient);

            builder.RegisterInstance(JsonSettings);
            
            RegisterFeatures(builder);
            
            OutsideBuilder?.Invoke(builder);
            builder.RegisterBuildCallback(GlobalServices.Initialize);
        }

        private void RegisterFeatures(IContainerBuilder builder)
        {
        }
    }
}