using Features.Core.MergeSystem.Config;
using Features.Core.MergeSystem.Controller;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder, SupplyWeightsConfigProvider supplyWeightsConfigProvider)
        {
            builder.Register<IMergeController, MergeController>(Lifetime.Singleton);
            builder.RegisterInstance<ISupplyWeightsConfigProvider, SupplyWeightsConfigProvider>(supplyWeightsConfigProvider);
            builder.RegisterInstance(supplyWeightsConfigProvider.GetConfig());
        }
    }
}