using Features.Core.ProductionSystem.Providers;
using VContainer;

namespace Features.Core.ProductionSystem
{
    public static class ProductionSystemRegistration
    {
        public static void RegisterProductionSystem(this IContainerBuilder builder, ProductionConfigProvider productionConfigProvider)
        {
            builder.RegisterInstance<IProductionConfigProvider, ProductionConfigProvider>(productionConfigProvider);
            builder.RegisterInstance(productionConfigProvider.GetConfig());
            
            builder.Register<ProductionController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}