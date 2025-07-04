using System.Threading;
using Common.PlayerData;
using Common.UI;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;

namespace Features.Core.ProductionSystem
{
    public interface IProductionView : IView
    {
        void Initialize(IPlayerDataService playerDataService,
            IViewLoader<ItemView, ProductionRecipe.Reward> rewardsViewLoader,
            IViewLoader<RecipeComponentView> itemsViewLoader,
            IViewLoader<ItemView, string> rewardItemViewLoader,
            IViewLoader<RecipeItemView> recipeItemViewLoader,
            IViewLoader<IngredientItemView> ingredientItemViewLoader,
            IControllerResources controllerResources, CancellationToken token);
    }
}