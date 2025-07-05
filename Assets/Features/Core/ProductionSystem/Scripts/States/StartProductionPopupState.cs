using System.Threading;
using Common.PlayerData;
using Common.UI;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;

namespace Features.Core.ProductionSystem
{
    public class StartProductionPopupState : BasicViewState<IProductionView, StartProductionPopupPayload>
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly ICraftingController _craftingController;
        private readonly IViewLoader<IRecipeComponentView> _recipeComponentViewLoader;
        private readonly IViewLoader<IItemView, string> _rewardItemViewLoader;
        private readonly IViewLoader<IRecipeItemView> _recipeItemViewLoader;
        private readonly IViewLoader<IIngredientItemView> _ingredientItemViewLoader;

        public StartProductionPopupState(ISharedViewLoader<IProductionView> sharedViewLoader,
            IPlayerDataService playerDataService,
            IViewLoader<IItemView, string> rewardsViewLoader,
            IViewLoader<IRecipeComponentView> recipeComponentViewLoader,
            IViewLoader<IRecipeItemView> recipeItemViewLoader,
            IViewLoader<IIngredientItemView> ingredientItemViewLoader) : base(sharedViewLoader)
        {
            _playerDataService = playerDataService;
            _recipeComponentViewLoader = recipeComponentViewLoader;
            _rewardItemViewLoader = rewardsViewLoader;
            _recipeItemViewLoader = recipeItemViewLoader;
            _ingredientItemViewLoader = ingredientItemViewLoader;
        }

        protected override async UniTask SetInitialViewState(StartProductionPopupPayload payload, IProductionView view,
            CancellationToken token)
        {
            view.Initialize(_playerDataService,
                async (key, container) =>
                    await _rewardItemViewLoader.Load(key, ControllerResources, token, container),
                async (container) =>
                    await _recipeComponentViewLoader.Load(ControllerResources, token, container),
                async (container) =>
                    await _recipeItemViewLoader.Load(ControllerResources, token, container),
                async (container) =>
                    await _ingredientItemViewLoader.Load(ControllerResources, token, container)
            );
        }

        protected override void SubscribeOnInput(IProductionView view)
        {
            view.OnStartProductionButtonPressedEvent += StartProduction;
        }

        protected override void UnsubscribeOnInput(IProductionView view)
        {
            view.OnStartProductionButtonPressedEvent -= StartProduction;
        }

        private void StartProduction(ProductionRecipe recipe)
        {
            _craftingController.StartCrafting(Payload.ProductionBuilding, recipe);
        }
    }
}