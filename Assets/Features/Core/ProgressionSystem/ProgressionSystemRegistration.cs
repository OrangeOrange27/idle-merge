using Features.Core.ProgressionSystem.Providers;
using VContainer;

namespace Features.Core.ProgressionSystem
{
    public static class ProgressionSystemRegistration
    {
        public static void RegisterProgressionSystem(this IContainerBuilder builder, ILevelsConfigProvider configProvider)
        {
            builder.RegisterInstance<ILevelsConfigProvider, LevelsConfigProvider>(configProvider);
            builder.RegisterInstance(configProvider.GetConfig());
            
            builder.Register<ProgressionManager>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}