using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Pooling.Infrastructure;
using Package.Disposables;
using UnityEngine;

namespace Package.AssetProvider.Pooling.Implementations
{
	public static class AssetPrefabsPoolExtensions
	{
		public static async UniTask Prewarm<T>(this IAssetPrefabsPool<T> prefabsPool, string key, int size, ICollection<IDisposable> disposables,
			CancellationToken cancellationToken) where T : Component
		{
			var reference = await prefabsPool.GetReference(key, size, cancellationToken);
			disposables.Add(reference);
		}

		public static async UniTask<T> Get<T>(this IAssetPrefabsPool<T> prefabsPool, string key, ICollection<IDisposable> disposables, CancellationToken cancellationToken,
			Transform parent = null) where T : Component
		{
			var reference = await prefabsPool.GetReference(key, 1, cancellationToken);

			T item;

			try
			{
				item = reference.Get(parent);
			}
			catch (Exception)
			{
				reference.Dispose();

				throw;
			}

			var disposable = new CompositeDisposable();
			disposable.Add(new Disposable(() =>
			{
				reference.Return(item);
				reference.Dispose();
			}));
			disposables.Add(disposable);

			return item;
		}
	}
}