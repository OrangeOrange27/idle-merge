using Features.Core.MergeSystem.Config;
using VContainer;

namespace Features.Core.MergeSystem
{
    public static class MergeSystemRegistration
    {
        public static void RegisterMergeSystem(this IContainerBuilder builder, SupplyWeightsConfigProvider supplyWeightsConfigProvider)
        {
            builder.RegisterInstance<ISupplyWeightsConfigProvider, SupplyWeightsConfigProvider>(supplyWeightsConfigProvider);
            builder.RegisterInstance(supplyWeightsConfigProvider.GetConfig());
        }
    }
}