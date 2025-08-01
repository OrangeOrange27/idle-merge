using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using TMPro;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookSectionView : MonoBehaviour, IDiscoveryBookSectionView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform[] _viewHolders;

        private List<IDiscoveryBookItemView> _spawnedViews;

        public async UniTask Initialize(DiscoveryBookSectionData sectionData,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> itemViewGetter,
            CancellationToken cancellationToken)
        {
            _title.text = sectionData.Title;
                
                //todo: handle progressive view

            await SpawnItemViews(sectionData.Items, itemViewGetter, cancellationToken);
        }

        private async UniTask SpawnItemViews(List<DiscoveryBookItemData> items,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            var taskList = new List<UniTask>();
            _spawnedViews = new List<IDiscoveryBookItemView>();

            foreach (var item in items)
            {
                taskList.Add(CreateItemView(item, rewardsViewGetter, cancellationToken));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.WhenAll(taskList);
        }

        private async UniTask CreateItemView(DiscoveryBookItemData item,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var view = await rewardsViewGetter(item.ViewKey, GetNextItemHolder());
            cancellationToken.ThrowIfCancellationRequested();
            
            view.SetText(item.Text);
            
                //todo: handle item unlock status
            
            _spawnedViews.Add(view);
        }

        private Transform GetNextItemHolder()
        {
            throw new NotImplementedException();
        }
    }
}