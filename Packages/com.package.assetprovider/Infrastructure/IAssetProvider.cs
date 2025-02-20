using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Package.AssetProvider.Infrastructure
{
	public interface IAssetProvider : IDisposable
	{
		UniTask Initialize();

		UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : Object;

		UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default)
			where T : Object;

		UniTask PreloadAssetsAsync(IProgress<DownloadStatus> progress, CancellationToken token = default,
			params string[] keys);

		UniTask<long> GetAssetsDownloadSize(CancellationToken token = default, params string[] keys);

		string[] GetAllKeys();
	}
}
