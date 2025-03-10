using Features.Core.MergeSystem.Providers;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder, MergesConfigProvider mergesConfigProvider)
        {
            builder.RegisterInstance<IMergesConfigProvider, MergesConfigProvider>(mergesConfigProvider);
            builder.RegisterInstance(mergesConfigProvider.GetConfig());
            
            builder.Register<MergeController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MergeProvider>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}