using Features.Core.Common.Views;
using UnityEngine;

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
        
        [SerializeField] private Transform _rewardsContainer;
        [SerializeField] private Transform _availableIndicator;
        [SerializeField] private Transform _inProgressIndicator;

        public Transform RewardsContainer => _rewardsContainer;

        public void SetRecipeStatus(Status status)
        {
            _availableIndicator.gameObject.SetActive(status == Status.Available);
            _inProgressIndicator.gameObject.SetActive(status == Status.InProgress);
        }
    }
}