using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.ProductionSystem.Components;
using Features.Core.SupplySystem.Models;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookTabView : MonoBehaviour
    {
        [SerializeField] DiscoveryBookTabType _tabType;
        
        public DiscoveryBookTabType TabType => _tabType;

        public async UniTask Initialize(MergeableObjectConfig config,
            Func<string, Transform, UniTask<IMergeableItemView>> rewardsViewGetter, CancellationToken cancellationToken)
        {
            
        }
    }
}