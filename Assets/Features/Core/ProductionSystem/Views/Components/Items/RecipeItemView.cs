using System;
using Features.Core.Common.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class RecipeItemView : ItemView
    {
        public enum Status
        {
            NotAvailable = 0,
            Available = 1,
            InProgress = 2,
        }
        
        [SerializeField] private Button _button;
        
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private Transform _rewardItemContainer;
        
        [SerializeField] private Transform _availableIndicator;
        [SerializeField] private Transform _inProgressIndicator;

        public Transform RewardItemContainer => _rewardItemContainer;
        public Transform RewardsContainer => _rewardsContainer;
        
        public event Action OnClick;

        public void SetRecipeStatus(Status status)
        {
            _availableIndicator.gameObject.SetActive(status == Status.Available);
            _inProgressIndicator.gameObject.SetActive(status == Status.InProgress);
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