using Common.Config;
using Features.Core.MergeSystem.Models;
using Features.Core.MergeSystem.Providers;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder)
        {
            builder.RegisterConfig<MergesConfig>("merges_config");
            builder.Register<MergeController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<MergeProvider>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}