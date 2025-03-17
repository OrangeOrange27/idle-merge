using Features.Core.ProductionSystem.Providers;
using VContainer;

namespace Features.Core.ProductionSystem
{
    public static class ProductionSystemRegistration
    {
        public static void RegisterProductionSystem(this IContainerBuilder builder,
            ProductionConfigProvider productionConfigProvider, RecycleConfigProvider recycleConfigProvider)
        {
            builder.RegisterInstance<IProductionConfigProvider, ProductionConfigProvider>(productionConfigProvider);
            builder.RegisterInstance(productionConfigProvider.GetConfig());

            builder.RegisterInstance<IRecycleConfigProvider, RecycleConfigProvider>(recycleConfigProvider);
            builder.RegisterInstance(productionConfigProvider.GetConfig());

            builder.Register<ProductionController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}