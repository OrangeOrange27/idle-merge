using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using AssetProvider.Implementation;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Infrastructure;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Package.AssetProvider.Implementation
{
	public class AddressablesAssetProvider : IAssetProvider
	{
		private const int AddressablesAssetProviderReleaseHandleDelayFramesCount = 1;
		
		private readonly IAssetProviderAnalyticsCallbacks _assetProviderAnalyticsCallbacks;

		private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();
		private readonly Dictionary<string, HashSet<string>> _cacheLabels = new Dictionary<string, HashSet<string>>();
		private readonly List<AsyncOperationHandle> _getDownloadSizeAsyncHandles = new List<AsyncOperationHandle>();

		private readonly Dictionary<string, AsyncOperationHandle> _handles =
			new Dictionary<string, AsyncOperationHandle>();

		private string[] _allKeys;

		public AddressablesAssetProvider(IAssetProviderAnalyticsCallbacks assetProviderAnalyticsCallbacks)
		{
			_assetProviderAnalyticsCallbacks = assetProviderAnalyticsCallbacks;
		}

		public async UniTask Initialize()
		{
			await InitializeAddressables();
		}

		public string[] GetAllKeys()
		{
			if (_allKeys == null)
				_allKeys = Addressables.ResourceLocators.SelectMany(locator => locator.Keys).Cast<string>().ToArray();

			return _allKeys;
		}

		public async UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : Object
		{
			if (string.IsNullOrEmpty(key)) throw new ArgumentNullException(nameof(key));

			if (_cache.ContainsKey(key)) return (T)GetCachedItem(key);

			if (_handles.ContainsKey(key))
			{
				var operationHandle = _handles[key];

				try
				{
					await operationHandle.WithCancellation(token);

					return (T)operationHandle.Result;
				}
				catch (OperationCanceledException)
				{
					throw;
				}
				catch (Exception e)
				{
					throw new AssetProviderException(key, e);
				}
			}

			var isKeyValid = await IsKeyValid(key);

			if (!isKeyValid) throw new Exception($"Key doesn't exist in addressables: {key}");

			var asyncOperationHandle = Addressables.LoadAssetAsync<T>(key);
			_handles.Add(key, asyncOperationHandle);

			try
			{
				var result = await asyncOperationHandle.WithCancellation(token);
				token.ThrowIfCancellationRequested();
				AddToCache(result, key);

				return result;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				throw new AssetProviderException(key, e);
			}
		}

		public async UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default)
			where T : Object
		{
			var result = await UniTask.WhenAll(Enumerable.Select(keys, key => LoadAsync<T>(key, token)));

			return result;
		}

		public async UniTask PreloadAssetsAsync(IProgress<DownloadStatus> progress, CancellationToken token = default,
			params string[] keys)
		{
			if (keys.Length == 0)
				return;

			var keysFormatted = GetKeysFormatted(keys);
			var totalSizeToDownload = await GetAssetsDownloadSize(token, keys);
			var startTime = Time.unscaledTime;
			_assetProviderAnalyticsCallbacks.OnRemoteAssetsPreloadStarted(totalSizeToDownload, keysFormatted);

			try
			{
				var downloadDependenciesAsync =
					Addressables.DownloadDependenciesAsync((IEnumerable<string>)keys, Addressables.MergeMode.Union);
				await CustomAddressablesAsyncExtensions.ToUniTask(downloadDependenciesAsync, progress,
					PlayerLoopTiming.Update, updateProgresEveryNFrames: 5, token);
				Addressables.Release(downloadDependenciesAsync);
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				_assetProviderAnalyticsCallbacks.OnRemoteAssetsPreloadFailed(e, keysFormatted);

				throw new AssetProviderException(keysFormatted, e);
			}

			var duration = Time.unscaledTime - startTime;
			_assetProviderAnalyticsCallbacks.OnRemoteAssetsPreloadFinished(totalSizeToDownload, keysFormatted, TimeSpan.FromSeconds(duration));
		}

		public async UniTask<long> GetAssetsDownloadSize(CancellationToken token = default, params string[] keys)
		{
			if (keys == null)
				return 0;
			
			var asyncOperationHandle = Addressables.GetDownloadSizeAsync((IEnumerable<string>)keys);
			_getDownloadSizeAsyncHandles.Add(asyncOperationHandle);

			return await asyncOperationHandle.WithCancellation(token);
		}

		public void Dispose()
		{
			ReleaseAllAssetsWithFrameDelay().Forget();
		}

		//We have to release asset after destroy GameObject of asset
		private async UniTaskVoid ReleaseAllAssetsWithFrameDelay()
		{
			await UniTask.DelayFrame(AddressablesAssetProviderReleaseHandleDelayFramesCount,
				PlayerLoopTiming.LastPostLateUpdate);
			await UniTask.SwitchToMainThread();

			foreach (var handleValue in _handles.Values) Addressables.Release(handleValue);

			foreach (var getDownloadSizeAsyncHandle in _getDownloadSizeAsyncHandles)
			{
				if (!getDownloadSizeAsyncHandle.IsValid()) continue;

				Addressables.Release(getDownloadSizeAsyncHandle);
			}

			_cacheLabels.Clear();
			_cache.Clear();
			_handles.Clear();
			_getDownloadSizeAsyncHandles.Clear();
		}

		private async UniTask InitializeAddressables()
		{
			await Addressables.InitializeAsync();
		}

		private object GetCachedItem(string key)
		{
			if (_cache.TryGetValue(key, out var value)) return value;

			return null;
		}

		private void AddToCache(object obj, string key, string label = "")
		{
			if (_cache.ContainsKey(key))
				_cache[key] = obj;
			else
				_cache.Add(key, obj);

			if (!string.IsNullOrEmpty(label))
			{
				if (_cacheLabels.TryGetValue(label, out var listKeys))
				{
					if (!listKeys.Contains(key)) listKeys.Add(key);
				}
				else
				{
					_cacheLabels.Add(label, new HashSet<string> { key });
				}
			}
		}

		private async UniTask<bool> IsKeyValid(string key)
		{
			var locations = await Addressables.LoadResourceLocationsAsync(key);

			return locations.Count > 0;
		}

		private string GetKeysFormatted(string[] keys)
		{
			if (keys == _allKeys)
				return "AllKeys";
			return string.Join(", ", keys);
		}
	}
}
