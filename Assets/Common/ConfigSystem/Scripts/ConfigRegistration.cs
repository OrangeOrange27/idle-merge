using System;
using Common.Config.Infrastructure;
using VContainer;

namespace Common.Config
{
    public static class ConfigRegistration
    {
        public static void RegisterConfig<T>(this IContainerBuilder container, string builtInConfigKey, Uri remoteConfigUrl) where T : BaseConfig
        {
            container.Register<RemoteConfigProvider<T>>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(builtInConfigKey)
                .WithParameter(remoteConfigUrl);
        }
        
        public static void RegisterConfig<T>(this IContainerBuilder container, string builtInConfigKey) where T : BaseConfig
        {
            container.Register<LocalConfigProvider<T>>(Lifetime.Singleton).AsImplementedInterfaces().WithParameter(builtInConfigKey);
        }
    }
}
