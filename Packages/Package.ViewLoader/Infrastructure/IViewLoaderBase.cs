using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Infrastructure
{
    public interface IViewLoaderBase
    {
        UniTask Prewarm(ICollection<IDisposable> resources, CancellationToken cancellationToken, int size = 1);
        UniTask<object> LoadDefault(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
    }
}