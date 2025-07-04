using System;
using Features.Core.Common.Views;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public interface IRecipeItemView : IItemView
    {
        public enum Status
        {
            NotAvailable = 0,
            Available = 1,
            InProgress = 2,
        }
        
        event Action OnClick;
        Transform RewardItemContainer { get; }
        Transform RewardsContainer { get; }
        
        void SetRecipeStatus(Status status);
    }
}