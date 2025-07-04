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
        private readonly IViewLoader<ItemView, ProductionRecipe.Reward> _rewardsViewLoader;
        private readonly IViewLoader<RecipeComponentView> _recipeComponentViewLoader;
        private readonly IViewLoader<ItemView, string> _rewardItemViewLoader;
        private readonly IViewLoader<RecipeItemView> _recipeItemViewLoader;
        private readonly IViewLoader<IngredientItemView> _ingredientItemViewLoader;

        public StartProductionPopupState(ISharedViewLoader<IProductionView> sharedViewLoader,
            IPlayerDataService playerDataService,
            IViewLoader<ItemView, ProductionRecipe.Reward> rewardsViewLoader,
            IViewLoader<RecipeComponentView> recipeComponentViewLoader,
            IViewLoader<ItemView, string> rewardItemViewLoader,
            IViewLoader<RecipeItemView> recipeItemViewLoader,
            IViewLoader<IngredientItemView> ingredientItemViewLoader) : base(sharedViewLoader)
        {
            _playerDataService = playerDataService;
            _rewardsViewLoader = rewardsViewLoader;
            _recipeComponentViewLoader = recipeComponentViewLoader;
            _rewardItemViewLoader = rewardItemViewLoader;
            _recipeItemViewLoader = recipeItemViewLoader;
            _ingredientItemViewLoader = ingredientItemViewLoader;
        }

        protected override async UniTask SetInitialViewState(StartProductionPopupPayload payload, IProductionView view,
            CancellationToken token)
        {
            view.Initialize(_playerDataService,
                async (key, container) =>
                    await _rewardsViewLoader.Load(key, ControllerResources, token, container),
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
            throw new System.NotImplementedException();
        }
    }
}