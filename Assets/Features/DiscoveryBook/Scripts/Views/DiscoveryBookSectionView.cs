using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.ProductionSystem.Components;
using Features.Core.SupplySystem.Models;
using TMPro;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookSectionView : MonoBehaviour, IDiscoveryBookSectionView
    {
        [SerializeField] private TMP_Text _title;
        [SerializeField] private Transform[] _viewHolders;

        private List<IMergeableItemView> _spawnedViews;
        private MergeableObjectConfig _config;

        public async UniTask Initialize(MergeableObjectConfig config,
            Func<string, Transform, UniTask<IMergeableItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            _config = config;
            _title.text = config.MergeableType.ToString();
            
            await SpawnMergeableViews(rewardsViewGetter, cancellationToken);
        }

        private async UniTask SpawnMergeableViews(
            Func<string, Transform, UniTask<IMergeableItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            var taskList = new List<UniTask>();
            _spawnedViews = new List<IMergeableItemView>();

            for (var i = 0; i < 4; i++)
            {
                taskList.Add(CreateMergeableView(i, rewardsViewGetter, cancellationToken));
            }

            cancellationToken.ThrowIfCancellationRequested();

            await UniTask.WhenAll(taskList);
        }

        private async UniTask CreateMergeableView(int stage,
            Func<string, Transform, UniTask<IMergeableItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var view = await rewardsViewGetter(_config.MergeableType.ToString(), _viewHolders[stage]);
            cancellationToken.ThrowIfCancellationRequested();
            
            view.SetStage(stage + 1);
            
            _spawnedViews.Add(view);
        }
    }
}