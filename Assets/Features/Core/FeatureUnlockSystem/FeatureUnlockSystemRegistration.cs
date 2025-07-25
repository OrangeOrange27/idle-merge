using Common.Config;
using VContainer;

namespace Features.Core
{
    public static class FeatureUnlockSystemRegistration
    {
        public static void RegisterFeatureUnlockSystem(this IContainerBuilder builder)
        {
            builder.RegisterConfig<FeaturesUnlockConfig>("unlock_config");

            builder.Register<IFeatureUnlockManager, FeatureUnlockManager>(Lifetime.Singleton);
        }
    }
}