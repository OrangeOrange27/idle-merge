using System;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class RecipeComponentView : ItemView
    {
        [SerializeField] private Button _hintButton;
        
        public event Action OnHintButtonPressedEvent;

        public void SetCollectibleType(CollectibleType collectibleType)
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            _hintButton.onClick.AddListener(OnHintButtonPressed);
        }

        private void OnDestroy()
        {
            _hintButton.onClick.RemoveListener(OnHintButtonPressed);
        }

        private void OnHintButtonPressed()
        {
            OnHintButtonPressedEvent?.Invoke();
        }
    }
}