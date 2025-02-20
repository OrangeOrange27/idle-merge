using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Tests.EditorAssetsCache;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Infrastructure;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace Tools.EditorTools.Mocs
{
	public class EditorAssetProvider : IAssetProvider
	{
		public UniTask Initialize()
		{
			return UniTask.CompletedTask;
		}

		public virtual UniTask<T> LoadAsync<T>(string key, CancellationToken token = default) where T : Object
		{
			key = TryTransformSpriteAtlasKey(key);

			var loadSingleAsset = EditorAddressableAssetSharedProvider.Instance.LoadAssetAsync<T>(key);

			return new UniTask<T>(loadSingleAsset);
		}

		public async UniTask<IEnumerable<T>> LoadAsync<T>(IEnumerable<string> keys, CancellationToken token = default) where T : Object
		{
			var result = await UniTask.WhenAll(Enumerable.Select(keys, key => LoadAsync<T>(key, token)));

			return result;
		}

		public UniTask PreloadAssetsAsync(IProgress<DownloadStatus> progress, CancellationToken token = default, params string[] keys)
		{
			return UniTask.CompletedTask;
		}

		public UniTask<long> GetAssetsDownloadSize(CancellationToken token = default, params string[] keys)
		{
			return UniTask.FromResult(0L);
		}

		public string[] GetAllKeys()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
		}

		private static string TryTransformSpriteAtlasKey(string key)
		{
			if (key.Contains("["))
			{
				var startIndex = key.IndexOf('[') + 1;
				key = key.Substring(startIndex, key.Length - startIndex - 1);
			}

			return key;
		}
	}
}