using System;
using System.Collections.Generic;
using System.Threading;
using Common.AssetProvider.Infrastructure;
using Common.AssetProvider.Pooling.Tests.Data;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Common.AssetProvider.ViewLoader.Tests.Mock
{
	public class ViewLoaderAssetProviderMock : IAssetProvider
	{
		private readonly CompositeDisposable _resources = new CompositeDisposable();

		public void Dispose()
		{
			_resources.Dispose();
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

			var gameObject = new GameObject("ViewLoaderAssetProviderMockTestObject");
			gameObject.AddComponent<TestView>();
			_resources.Add(gameObject.AsDisposable());

			return UniTask.FromResult(gameObject as T);
		}

		public UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default) where T : UnityEngine.Object
		{
			throw new System.NotImplementedException();
		}

		public UniTask<long> GetAssetsDownloadSize(CancellationToken token = default, params string[] keys)
		{
			throw new System.NotImplementedException();
		}

		public UniTask PreloadAssetsAsync(IProgress<DownloadStatus> progress, CancellationToken token = default, params string[] keys)
		{
			throw new System.NotImplementedException();
		}
	}
}