using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class ProductionOrdersTabView : MonoBehaviour
    {
        [SerializeField] private Button _startProductionButton;
        [SerializeField] private ProductionRecipeViewController _recipeView;
        
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private Transform _recipesContainer;

        private IViewLoader<ItemView, ProductionRecipe.Reward> _rewardsViewLoader;
        private IViewLoader<RecipeItemView, string> _recipeItemViewLoader;
        private IControllerResources _controllerResources;
        private CancellationToken _cancellationToken;
        
        private Dictionary<ProductionRecipe, RecipeItemView> _recipeToViewMap = new();
        private List<ItemView> _spawnedRewardItems = new();
        private ProductionRecipe _selectedRecipe;
        
        public event Action<ProductionRecipe> OnStartProductionButtonPressedEvent;

        public void Initialize(IPlayerDataService playerDataService, 
            IViewLoader<ItemView, ProductionRecipe.Reward> rewardsViewLoader,
            IViewLoader<RecipeComponentView, CollectibleType> itemsViewLoader,
            IViewLoader<ItemView, string> rewardItemViewLoader, 
            IViewLoader<RecipeItemView, string> recipeItemViewLoader, 
            IControllerResources controllerResources, CancellationToken token)
        {
            _rewardsViewLoader = rewardsViewLoader;
            _recipeItemViewLoader = recipeItemViewLoader;
            _controllerResources = controllerResources;
            _cancellationToken = token;

            _startProductionButton.onClick.AddListener(OnStartButtonPressed);

            _recipeView.Initialize(playerDataService, itemsViewLoader, rewardItemViewLoader);
        }
        
        public async UniTask CreateRecipeViews(IEnumerable<ProductionRecipe> recipes)
        {
            foreach (var recipeView in _recipeToViewMap.Values)
            {
                if(recipeView!=null)
                    Destroy(recipeView.gameObject);
            }
            _recipeToViewMap.Clear();
            
            foreach (var recipe in recipes)
            {
                var recipeView = await _recipeItemViewLoader.Load(recipe.RecipeName, _controllerResources,
                    _cancellationToken, _recipesContainer);
                
                var recipeRewardItemView = await _recipeItemViewLoader.Load(recipe.RecipeName, _controllerResources,
                    _cancellationToken, recipeView.RewardItemContainer);

                foreach (var reward in recipe.Outcome)
                {
                    var rewardView = await _rewardsViewLoader.Load(reward, _controllerResources, _cancellationToken,
                        recipeView.RewardsContainer);
                }

                recipeView.OnClick += () => SetRecipe(recipe).Forget();
                
                _recipeToViewMap.Add(recipe, recipeView);
            }
        }

        private async UniTask SetRecipe(ProductionRecipe recipe)
        {
            _selectedRecipe = recipe;
            await UniTask.WhenAll(SetRecipeRewards(recipe),
                _recipeView.SetRecipe(recipe, _controllerResources, _cancellationToken));
        }

        private async UniTask SetRecipeRewards(ProductionRecipe recipe)
        {
            ClearRewardItems();
            foreach (var reward in recipe.Outcome)
            {
                var rewardView = await _rewardsViewLoader.Load(reward, _controllerResources, _cancellationToken,
                    _rewardsContainer);
                
                _spawnedRewardItems.Add(rewardView);
            }
        }
        
        private void OnStartButtonPressed()
        {
            OnStartProductionButtonPressedEvent?.Invoke(_selectedRecipe);
        }

        private void ClearRewardItems()
        {
            foreach (var rewardItem in _spawnedRewardItems)
            {
                if(rewardItem != null)
                    Destroy(rewardItem.gameObject);
            }
            _spawnedRewardItems.Clear();
        }

        private void OnDestroy()
        {
            _startProductionButton.onClick.RemoveAllListeners();
        }
    }
}