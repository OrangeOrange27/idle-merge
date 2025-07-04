using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Models;
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

        private Func<ProductionRecipe.Reward, Transform, UniTask<ItemView>> _rewardViewGetter;
        private Func<Transform, UniTask<RecipeItemView>> _recipeItemViewGetter;
        private Func<string, Transform, UniTask<ItemView>> _rewardItemViewGetter;
        
        private Dictionary<ProductionRecipe, RecipeItemView> _recipeToViewMap = new();
        private List<ItemView> _spawnedRewardItems = new();
        private ProductionRecipe _selectedRecipe;
        
        public event Action<ProductionRecipe> OnStartProductionButtonPressedEvent;

        public void Initialize(IPlayerDataService playerDataService,
            Func<ProductionRecipe.Reward, Transform, UniTask<ItemView>> rewardViewGetter,
            Func<string, Transform, UniTask<ItemView>> rewardItemViewGetter,
            Func<Transform, UniTask<RecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<RecipeItemView>> recipeItemViewGetter)
        {
            _rewardViewGetter = rewardViewGetter;
            _rewardItemViewGetter = rewardItemViewGetter;
            _recipeItemViewGetter = recipeItemViewGetter;
            
            _startProductionButton.onClick.AddListener(OnStartButtonPressed);

            _recipeView.Initialize(playerDataService, recipeComponentViewGetter, rewardItemViewGetter);
        }
        
        public async UniTask CreateRecipeViews(IEnumerable<ProductionRecipe> recipes, CancellationToken token)
        {
            foreach (var recipeView in _recipeToViewMap.Values)
            {
                if(recipeView!=null)
                    Destroy(recipeView.gameObject);
            }
            _recipeToViewMap.Clear();
            
            foreach (var recipe in recipes)
            {
                var recipeView = await _recipeItemViewGetter.Invoke(_recipesContainer);
                
                var recipeRewardItemView = await _rewardItemViewGetter.Invoke(recipe.RecipeName, recipeView.RewardItemContainer);

                foreach (var reward in recipe.Outcome)
                {
                    var rewardView = await _rewardViewGetter.Invoke(reward, recipeView.RewardsContainer);
                }

                recipeView.OnClick += () => SetRecipe(recipe, token).Forget();
                
                _recipeToViewMap.Add(recipe, recipeView);
            }
        }

        private async UniTask SetRecipe(ProductionRecipe recipe, CancellationToken token)
        {
            _selectedRecipe = recipe;
            await UniTask.WhenAll(SetRecipeRewards(recipe, token),
                _recipeView.SetRecipe(recipe, token));
        }

        private async UniTask SetRecipeRewards(ProductionRecipe recipe, CancellationToken token)
        {
            ClearRewardItems();
            foreach (var reward in recipe.Outcome)
            {
                token.ThrowIfCancellationRequested();
                var rewardView = await _rewardViewGetter.Invoke(reward, _rewardsContainer);
                token.ThrowIfCancellationRequested();
                
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