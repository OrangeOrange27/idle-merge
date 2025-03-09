using Features.Core.SupplySystem.Providers;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder, SupplyWeightsConfigProvider supplyWeightsConfigProvider)
        {
            builder.RegisterInstance<ISupplyWeightsConfigProvider, SupplyWeightsConfigProvider>(supplyWeightsConfigProvider);
            builder.RegisterInstance(supplyWeightsConfigProvider.GetConfig());

            builder.Register<MergeController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}