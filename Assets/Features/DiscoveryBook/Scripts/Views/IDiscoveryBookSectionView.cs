using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public interface IDiscoveryBookSectionView
    {
        UniTask Initialize(DiscoveryBookSectionData sectionData,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> itemViewGetter,
            CancellationToken cancellationToken);
    }
}