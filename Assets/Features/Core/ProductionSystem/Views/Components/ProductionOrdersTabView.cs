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

        private Func<Transform, UniTask<IRecipeItemView>> _recipeItemViewGetter;
        private Func<string, Transform, UniTask<IItemView>> _rewardItemViewGetter;
        
        private Dictionary<ProductionRecipe, IRecipeItemView> _recipeToViewMap = new();
        private List<IItemView> _spawnedRewardItems = new();
        private ProductionRecipe _selectedRecipe;
        
        public event Action<ProductionRecipe> OnStartProductionButtonPressedEvent;

        public void Initialize(IPlayerDataService playerDataService,
            Func<string, Transform, UniTask<IItemView>> rewardItemViewGetter,
            Func<Transform, UniTask<IRecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<IRecipeItemView>> recipeItemViewGetter)
        {
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
                    Destroy(recipeView.GameObject);
            }
            _recipeToViewMap.Clear();
            
            foreach (var recipe in recipes)
            {
                var recipeView = await _recipeItemViewGetter.Invoke(_recipesContainer);
                
                var recipeRewardItemView = await _rewardItemViewGetter.Invoke(recipe.RecipeName, recipeView.RewardItemContainer);

                foreach (var reward in recipe.Outcome)
                {
                    var rewardView = await _rewardItemViewGetter.Invoke(reward.GetViewKey(), recipeView.RewardsContainer);
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
                var rewardView = await _rewardItemViewGetter.Invoke(reward.GetViewKey(), _rewardsContainer);
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
                    Destroy(rewardItem.GameObject);
            }
            _spawnedRewardItems.Clear();
        }

        private void OnDestroy()
        {
            _startProductionButton.onClick.RemoveAllListeners();
        }
    }
}