using System;
using Features.Core.Common.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class RecipeItemView : ItemView, IRecipeItemView
    {
        [SerializeField] private Button _button;
        
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private Transform _rewardItemContainer;
        
        [SerializeField] private Transform _availableIndicator;
        [SerializeField] private Transform _inProgressIndicator;

        public Transform RewardItemContainer => _rewardItemContainer;
        public Transform RewardsContainer => _rewardsContainer;
        
        public event Action OnClick;

        public void SetRecipeStatus(IRecipeItemView.Status status)
        {
            _availableIndicator.gameObject.SetActive(status == IRecipeItemView.Status.Available);
            _inProgressIndicator.gameObject.SetActive(status == IRecipeItemView.Status.InProgress);
        }

        private void Awake()
        {
            _button.onClick.AddListener(() => OnClick?.Invoke());
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}