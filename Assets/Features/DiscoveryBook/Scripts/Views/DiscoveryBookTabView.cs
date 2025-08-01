using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookTabView : MonoBehaviour
    {
        [SerializeField] DiscoveryBookTabType _tabType;

        public DiscoveryBookTabType TabType => _tabType;

        private Func<string, Transform, UniTask<IDiscoveryBookItemView>> _itemViewGetter;

        public async UniTask Initialize(List<DiscoveryBookSectionData> payload,
            Func<Transform, UniTask<IDiscoveryBookSectionView>> sectionViewGetter,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> itemViewGetter,
            CancellationToken cancellationToken)
        {
            _itemViewGetter = itemViewGetter;

            await SpawnSections(payload, sectionViewGetter, cancellationToken);
        }

        private async UniTask SpawnSections(List<DiscoveryBookSectionData> payload,
            Func<Transform, UniTask<IDiscoveryBookSectionView>> sectionViewGetter, CancellationToken cancellationToken)
        {
            var taskList = Enumerable.Select(payload,
                sectionData => CreateSection(sectionData, sectionViewGetter, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.WhenAll(taskList);
        }

        private async UniTask CreateSection(DiscoveryBookSectionData sectionData,
            Func<Transform, UniTask<IDiscoveryBookSectionView>> sectionViewGetter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sectionView = await sectionViewGetter(transform);
            cancellationToken.ThrowIfCancellationRequested();

            await sectionView.Initialize(sectionData, _itemViewGetter, cancellationToken);
        }
    }
}