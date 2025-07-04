using System;
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
            Func<ProductionRecipe.Reward, Transform, UniTask<ItemView>> rewardViewGetter, 
            Func<string, Transform, UniTask<ItemView>> rewardItemViewGetter,
            Func<Transform, UniTask<RecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<RecipeItemView>> recipeItemViewGetter, 
            Func<Transform, UniTask<IngredientItemView>> ingredientItemViewGetter)
        {
            _closeButton.onClick.AddListener(OnCloseButtonPressed);
            _ordersButton.onClick.AddListener(() => SwitchTab(TabType.Orders));
            _ingredientsButton.onClick.AddListener(() => SwitchTab(TabType.Ingredients));

            _ordersTab.Initialize(playerDataService, rewardViewGetter, rewardItemViewGetter, recipeComponentViewGetter,
                recipeItemViewGetter);
            
            _ordersTab.OnStartProductionButtonPressedEvent += OnStartProductionButtonPressed;

            _ingredientsTab.Initialize(ingredientItemViewGetter, playerDataService);
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