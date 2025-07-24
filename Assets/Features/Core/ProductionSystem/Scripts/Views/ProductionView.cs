using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem
{
    public class ProductionView : MonoBehaviour, IProductionView
    {
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _ordersButton;
        [SerializeField] private Button _ingredientsButton;

        [SerializeField] private TMP_Text _title;

        [SerializeField] private ProductionOrdersTabView _ordersTab;
        [SerializeField] private ProductionIngredientsTabView _ingredientsTab;

        public event Action OnCloseButtonPressedEvent;
        public event Action<ProductionRecipe> OnStartProductionButtonPressedEvent;

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(false);
        }

        public void Initialize(IPlayerDataService playerDataService,
            Func<string, Transform, UniTask<IItemView>> rewardItemViewGetter,
            Func<string, Transform, UniTask<IMergeableItemView>> rewardsViewGetter,
            Func<Transform, UniTask<IRecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<IRecipeItemView>> recipeItemViewGetter,
            Func<Transform, UniTask<IIngredientItemView>> ingredientItemViewGetter)
        {
            _closeButton.onClick.AddListener(OnCloseButtonPressed);
            _ordersButton.onClick.AddListener(() => SwitchTab(TabType.Orders));
            _ingredientsButton.onClick.AddListener(() => SwitchTab(TabType.Ingredients));

            _ordersTab.Initialize(playerDataService, rewardItemViewGetter, rewardsViewGetter, recipeComponentViewGetter,
                recipeItemViewGetter);

            _ordersTab.OnStartProductionButtonPressedEvent += OnStartProductionButtonPressed;

            _ingredientsTab.Initialize(ingredientItemViewGetter, playerDataService);
        }

        public async UniTask SetRecipes(IEnumerable<ProductionRecipe> recipes, CancellationToken token)
        {
            await _ordersTab.CreateRecipeViews(recipes, token);
        }

        private void OnCloseButtonPressed()
        {
            OnCloseButtonPressedEvent?.Invoke();
        }

        private void OnStartProductionButtonPressed(ProductionRecipe recipe)
        {
            OnStartProductionButtonPressedEvent?.Invoke(recipe);
        }

        private void SwitchTab(TabType tabType)
        {
            _ordersTab.gameObject.SetActive(tabType == TabType.Orders);
            _ingredientsTab.gameObject.SetActive(tabType == TabType.Ingredients);
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
            _ordersButton.onClick.RemoveAllListeners();
            _ingredientsButton.onClick.RemoveAllListeners();

            _ordersTab.OnStartProductionButtonPressedEvent -= OnStartProductionButtonPressed;
        }

        private enum TabType
        {
            Orders,
            Ingredients,
        }
    }
}