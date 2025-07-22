using Common.Config;
using Features.Core.SupplySystem.Models;
using Features.Core.SupplySystem.Providers;
using VContainer;

namespace Features.Core.SupplySystem
{
    public static class SupplySystemRegistration
    {
        public static void RegisterSupplySystem(this IContainerBuilder builder)
        {
            builder.RegisterConfig<SupplyWeightsConfig>("supply_weights_config");

            builder.Register<ISupplyManager, SupplyManager>(Lifetime.Singleton);
            builder.Register<ISupplyProvider, RandomSupplyProvider>(Lifetime.Singleton);
        }
    }
}