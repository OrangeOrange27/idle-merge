using System;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
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
        public event Action OnStartButtonPressedEvent;

        public void Initialize(IPlayerDataService playerDataService, 
            IViewLoader<ItemView, ProductionRecipe.Reward> rewardsViewLoader,
            IViewLoader<RecipeComponentView, CollectibleType> itemsViewLoader,
            IViewLoader<ItemView, string> rewardItemViewLoader, 
            IViewLoader<RecipeItemView, string> recipeItemViewLoader, 
            IControllerResources controllerResources, CancellationToken token)
        {
            _closeButton.onClick.AddListener(OnCloseButtonPressed);
            _ordersButton.onClick.AddListener(() => SwitchTab(TabType.Orders));
            _ingredientsButton.onClick.AddListener(() => SwitchTab(TabType.Ingredients));

            _ordersTab.Initialize(playerDataService, rewardsViewLoader, itemsViewLoader, rewardItemViewLoader,
                recipeItemViewLoader, controllerResources, token);
            
            _ordersTab.OnStartButtonPressedEvent += OnStartButtonPressed;
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(false);
        }

        private void OnCloseButtonPressed()
        {
            OnCloseButtonPressedEvent?.Invoke();
        }
        
        private void OnStartButtonPressed()
        {
            OnStartButtonPressedEvent?.Invoke();
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
            
            _ordersTab.OnStartButtonPressedEvent -= OnStartButtonPressed;
        }

        private enum TabType
        {
            Orders,
            Ingredients,
        }
    }
}