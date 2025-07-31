using System;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookTabButton : MonoBehaviour
    {
        [SerializeField] DiscoveryBookTabType _tabType;
        [SerializeField] Button _button;

        public event Action<DiscoveryBookTabType> OnClick;

        private void Awake()
        {
            _button.onClick.AddListener((() => OnClick?.Invoke(_tabType)));
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }
    }
}