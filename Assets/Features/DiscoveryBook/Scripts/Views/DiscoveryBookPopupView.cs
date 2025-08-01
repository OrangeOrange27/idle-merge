using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookPopupView : MonoBehaviour, IDiscoveryBookPopupView
    {
        [SerializeField] private DiscoveryBookTabView[] _tabs;
        [SerializeField] private DiscoveryBookTabButton[] _tabButtons;
        [SerializeField] private Button _closeButton;

        public event Action OnCloseButtonPressedEvent;

        private Dictionary<DiscoveryBookTabType, DiscoveryBookTabView> _tabsDictionary;
        private DiscoveryBookTabView _currentTab;

        public async UniTask Initialize(Dictionary<DiscoveryBookTabType, List<DiscoveryBookSectionData>> payload,
            Func<Transform, UniTask<IDiscoveryBookSectionView>> sectionViewGetter,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> itemViewGetter,
            CancellationToken cancellationToken)
        {
            BuildTabsDictionary();

            var tasks = Enumerable.Select(_tabsDictionary,
                tab => tab.Value.Initialize(payload[tab.Key], sectionViewGetter, itemViewGetter, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var tabButton in _tabButtons)
            {
                tabButton.OnClick += SwitchTab;
            }

            _closeButton.onClick.AddListener(() => OnCloseButtonPressedEvent?.Invoke());

            cancellationToken.ThrowIfCancellationRequested();
            await UniTask.WhenAll(tasks);
        }

        public async UniTask ShowAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(true);
        }

        public async UniTask HideAsync(CancellationToken cancellationToken)
        {
            gameObject.SetActive(false);
        }

        private void BuildTabsDictionary()
        {
            _tabsDictionary = new Dictionary<DiscoveryBookTabType, DiscoveryBookTabView>();

            foreach (var view in _tabs)
            {
                _tabsDictionary.Add(view.TabType, view);
            }
        }

        private void SwitchTab(DiscoveryBookTabType tabType)
        {
            _currentTab.gameObject.SetActive(false);
            _tabsDictionary[tabType].gameObject.SetActive(true);

            _currentTab = _tabsDictionary[tabType];
        }

        private void OnDestroy()
        {
            _closeButton.onClick.RemoveAllListeners();
        }
    }
}