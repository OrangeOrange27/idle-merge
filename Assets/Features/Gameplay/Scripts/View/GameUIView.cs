using System;
using Common.PlayerData;
using Features.Core.ProgressionSystem;
using Features.Gameplay.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Gameplay.View
{
    public class GameUIView : MonoBehaviour, IGameUIView
    {
        [SerializeField] private TopPanel _topPanel;
        [SerializeField] private Button _supplyButton;

        public event Action OnSupplyButtonClicked;

        public void Initialize(GameUIDTO dto)
        {
            _topPanel.Initialize(dto.PlayerDataService, dto.ProgressionManager);
            
            _supplyButton.onClick.AddListener(() => OnSupplyButtonClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _supplyButton.onClick.RemoveListener(() => OnSupplyButtonClicked?.Invoke());
        }
    }
}