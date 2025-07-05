using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Providers;
using Package.AssetProvider.ViewLoader.VContainer;
using Package.ControllersTree.VContainer;
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
            builder.RegisterInstance(recycleConfigProvider.GetConfig());

            builder.Register<ProductionController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<CraftingController>(Lifetime.Singleton).AsImplementedInterfaces();
            
            builder.RegisterController<StartProductionPopupState>();

            builder.RegisterViewLoader<RecipeComponentView, IRecipeComponentView>("RecipeComponentView");
            builder.RegisterViewLoader<RecipeItemView, IRecipeItemView>("RecipeItemView");
            builder.RegisterViewLoader<IngredientItemView, IIngredientItemView>("IngredientItemView");
            builder.RegisterViewLoader<ItemView, IItemView, string>(key => key);
        }
    }
}