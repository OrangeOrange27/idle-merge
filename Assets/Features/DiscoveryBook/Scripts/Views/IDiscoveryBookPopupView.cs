using System;
using System.Collections.Generic;
using System.Threading;
using Common.UI;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using UnityEngine;

namespace Features.DiscoveryBook.Scripts.Views
{
    public interface IDiscoveryBookPopupView : IView
    {
        UniTask Initialize(Dictionary<DiscoveryBookTabType, List<DiscoveryBookSectionData>> payload,
            Func<Transform, UniTask<IDiscoveryBookSectionView>> sectionViewGetter,
            Func<string, Transform, UniTask<IDiscoveryBookItemView>> itemViewGetter,
            CancellationToken cancellationToken);
    }
}