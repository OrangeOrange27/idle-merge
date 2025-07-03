using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.ProductionSystem.Components;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem
{
    public class ProductionView : MonoBehaviour, IProductionView
    {
        [SerializeField] private Button _startProductionButton;
        [SerializeField] private Button _closeButton;
        [SerializeField] private Button _ordersButton;
        [SerializeField] private Button _ingredientsButton;
        
        [SerializeField] private TMP_Text _title;
        [SerializeField] private ProductionRecipeView _recipeView;
        
        public event Action OnCloseButtonPressedEvent;

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(false);
        }
    }
}