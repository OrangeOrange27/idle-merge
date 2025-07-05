using System;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class RecipeComponentView : ItemView, IRecipeComponentView
    {
        [SerializeField] private Button _hintButton;
        [SerializeField] private TMP_Text _text;
        
        public event Action OnHintButtonPressedEvent;

        public void SetCollectibleType(CollectibleType collectibleType)
        {
            throw new NotImplementedException();
        }

        public void SetText(string text)
        {
            _text.text = text;
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