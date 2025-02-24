using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Infrastructure
{
    public interface ISharedViewLoaderBase
    {
        UniTask Prewarm(ICollection<IDisposable> resources, CancellationToken cancellationToken);
        UniTask<object> LoadDefault(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
    }
}