using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Infrastructure;
using Package.AssetProvider.Pooling.Exceptions;
using Package.AssetProvider.Pooling.Infrastructure;
using Package.Disposables;
using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.AssetProvider.Pooling.Implementations
{
	public sealed class AssetPrefabsPool<T> : IAssetPrefabsPool<T> where T : Component
	{
		private readonly IPrefabsPool<T> _prefabsPool;
		private readonly Func<IAssetProvider> _assetProviderFactory;

		private IAssetProvider _assetProvider;
		private int _referenceCount;

		public AssetPrefabsPool(IPrefabsPool<T> prefabsPool, Func<IAssetProvider> assetProviderFactory)
		{
			_prefabsPool = prefabsPool;
			_assetProviderFactory = assetProviderFactory;
		}

		public async UniTask<IPoolReference<T>> GetReference(string key, int size,
			CancellationToken cancellationToken = default)
		{
			_assetProvider ??= _assetProviderFactory.Invoke();

			PoolReferenceCreated();

			try
			{
				var component = await LoadPrefab(_assetProvider, key, cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();

				var innerPoolReference = _prefabsPool.GetPoolReference(component, size);

				return new Reference(innerPoolReference, new Disposable(PoolReferenceDestroyed));
			}
			catch (Exception)
			{
				PoolReferenceDestroyed();

				throw;
			}
		}

		private async UniTask<T> LoadPrefab(IAssetProvider assetProvider, string key,
			CancellationToken cancellationToken)
		{
			var prefab = await assetProvider.LoadAsync<GameObject>(key, cancellationToken);

			if (prefab == null)
				throw new AssetsPrefabsPoolProviderException(
					$"{nameof(assetProvider.LoadAsync)} returned null object for {key}");

			var component = prefab.GetComponent<T>();

			if (component == null)
				throw new AssetsPrefabsPoolProviderException(
					$"{nameof(assetProvider.LoadAsync)} returned prefab without {typeof(T).Name} component for '{key}'. {prefab.name} ({prefab.GetInstanceID()}). Is _assetProvide disposed: {_assetProvider == null}. If true, then somebody disposed reference during loading new one");

			return component;
		}

		private void PoolReferenceCreated()
		{
			_referenceCount++;
		}

		private void PoolReferenceDestroyed()
		{
			--_referenceCount;

			if (_referenceCount <= 0)
			{
				_assetProvider?.Dispose();
				_assetProvider = null;
			}
		}

		public sealed class Reference : IPoolReference<T>
		{
			private readonly IDisposable _poolHandle;
			private readonly IPoolReference<T> _prefabsPoolReference;

			private bool _isDisposed;

			internal Reference(IPoolReference<T> prefabsPoolReference, IDisposable poolHandle)
			{
				_prefabsPoolReference = prefabsPoolReference;
				_poolHandle = poolHandle;
			}

			public T Get(Transform parent = null)
			{
				EnsureNotDisposed();

				return _prefabsPoolReference.Get(parent);
			}

			public void Return(T pooledObject)
			{
				EnsureNotDisposed();

				_prefabsPoolReference.Return(pooledObject);
			}

			public void Dispose()
			{
				if (_isDisposed) return;

				_isDisposed = true;
				_poolHandle.Dispose();
				_prefabsPoolReference.Dispose();
			}

			private void EnsureNotDisposed()
			{
				if (_isDisposed) throw new ObjectDisposedException(nameof(Reference), "Already disposed");
			}
		}
	}
}
