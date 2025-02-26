using System;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Gameplay.View
{
    public class GameUIView : MonoBehaviour, IGameUIView
    {
        [SerializeField] private Button _supplyButton;

        private void Awake()
        {
            _supplyButton.onClick.AddListener(() => OnSupplyButtonClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _supplyButton.onClick.RemoveListener(() => OnSupplyButtonClicked?.Invoke());
        }

        public event Action OnSupplyButtonClicked;
    }
}