using System;
using System.Collections.Generic;
using System.Threading;
using Common.AssetProvider.Implementation;
using Common.AssetProvider.Infrastructure;
using Common.AssetProvider.Pooling.Tests.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.AssetProvider.Pooling.Tests.Mocks
{
	internal class AssetsPrefabsPoolTestsAssetProviderMock : IAssetProvider
	{
		private readonly TestAssetsProviderInstanceCounter _instanceCounter;
		private readonly CompositeDisposable _resources = new CompositeDisposable();
		private readonly int _id;

		private int _loadAttempts;
		private bool _isDisposed;

		public AssetsPrefabsPoolTestsAssetProviderMock(
			TestAssetsProviderInstanceCounter instanceCounter
		)
		{
			_instanceCounter = instanceCounter;
			_id = _instanceCounter.AssetProviderCreated();
		}

		public UniTask Initialize()
		{
			return UniTask.CompletedTask;
		}

		public UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : UnityEngine.Object
		{
			if (typeof(T) != typeof(GameObject))
			{
				throw new NotImplementedException();
			}

			_loadAttempts++;

			if (key == AssetPrefabsPoolTestAddresablesKey.NormalView)
			{
				var prefab = CreateNewGameObject(key);

				prefab.AddComponent<AssetPrefabsPoolTestView>();

				return UniTask.FromResult(prefab as T);
			}
			else if (key == AssetPrefabsPoolTestAddresablesKey.ViewThatCausesException)
			{
				throw new Exception("LoadAsync was failed intentionally!");
			}
			else if (key == AssetPrefabsPoolTestAddresablesKey.ViewThatNull)
			{
				return UniTask.FromResult<T>(null);
			}
			else if (key == AssetPrefabsPoolTestAddresablesKey.ViewWithoutComponent)
			{
				var prefab = CreateNewGameObject(key);

				return UniTask.FromResult(prefab as T);
			}

			throw new NotImplementedException($"LoadAsync doesn't support {key} view");
		}

		public UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default) where T : UnityEngine.Object
		{
			throw new NotImplementedException();
		}

		public UniTask PreloadAssetsAsync(IProgress<DownloadStatus> progress, CancellationToken token = default, params string[] keys)
		{
			throw new NotImplementedException();
		}

		public UniTask<long> GetAssetsDownloadSize(CancellationToken token = default, params string[] keys)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			_isDisposed = true;
			_resources.Dispose();
			_instanceCounter.AssetProviderDisposed();
		}

		private GameObject CreateNewGameObject(string key)
		{
			var gameObject = new GameObject($"{key}_{_id}_{_loadAttempts}");
			_instanceCounter.ViewInstanceCreated();

			_resources.Add(gameObject.AsDisposable());
			_resources.Add(new Disposable(_instanceCounter.ViewInstanceDestroyed));


			return gameObject;
		}
	}
}