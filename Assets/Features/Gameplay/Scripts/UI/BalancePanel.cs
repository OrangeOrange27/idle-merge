using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Gameplay.Scripts.UI
{
    public class BalancePanel : MonoBehaviour, IBalancePanel
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Button _button;
        
        public event Action OnButtonClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnButtonClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void SetBalance(int value)
        {
            _text.text = value.ToString();
        }
    }
}