using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Pooling.Implementations;
using Package.AssetProvider.Pooling.Infrastructure;
using Package.AssetProvider.ViewLoader.Infrastructure;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Implementation
{
	internal class ViewLoader<TViewPrefab, TViewInterface, TKey> : IViewLoader<TViewInterface>, IViewLoader<TViewInterface, TKey>
		where TViewPrefab : Component, TViewInterface where TViewInterface : class
	{
		private readonly Func<TKey, string> _convertKeyToAddressablesKey;
		private readonly IAssetPrefabsPool<TViewPrefab> _pool;

		public ViewLoader(IAssetPrefabsPool<TViewPrefab> pool, Func<TKey, string> convertKeyToAddressablesKey)
		{
			_pool = pool;
			_convertKeyToAddressablesKey = convertKeyToAddressablesKey;
		}

		public UniTask Prewarm(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, int size = 1)
		{
			return _pool.Prewarm(_convertKeyToAddressablesKey.Invoke(key), size, resources, cancellationToken);
		}

		public async UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent)
		{
			return await _pool.Get(_convertKeyToAddressablesKey.Invoke(key), resources, cancellationToken, parent);
		}

		public UniTask Prewarm(ICollection<IDisposable> resources, CancellationToken cancellationToken, int size)
		{
			return Prewarm(default, resources, cancellationToken);
		}

		public async UniTask<TViewInterface> Load(ICollection<IDisposable> resources,
			CancellationToken cancellationToken, Transform parent)
		{
			return await _pool.Get(_convertKeyToAddressablesKey.Invoke(default), resources, cancellationToken, parent);
		}

		public async UniTask<object> LoadDefault(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent)
		{
			return await Load(resources, cancellationToken, parent);
		}
	}
}
