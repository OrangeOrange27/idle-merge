using Features.Core.SupplySystem.Providers;
using VContainer;

namespace Features.Core.SupplySystem
{
    public static class SupplySystemRegistration
    {
        public static void RegisterSupplySystem(this IContainerBuilder builder, SupplyWeightsConfigProvider supplyWeightsConfigProvider)
        {
            builder.Register<ISupplyManager, SupplyManager>(Lifetime.Singleton);
            builder.Register<ISupplyProvider, RandomSupplyProvider>(Lifetime.Singleton);
            
            builder.RegisterInstance<ISupplyWeightsConfigProvider, SupplyWeightsConfigProvider>(supplyWeightsConfigProvider);
            builder.RegisterInstance(supplyWeightsConfigProvider.GetConfig());
        }
    }
}