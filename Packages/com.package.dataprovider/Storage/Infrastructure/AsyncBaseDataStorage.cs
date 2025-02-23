using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.DataProvider.Storage.Infrastructure
{
	public abstract class AsyncBaseDataStorage : IDataStorage
	{
		protected class CacheDataStruct
		{
			public string LastSaveCache;
			public string CurrentSaveCache;
			public string Data;
		}

		private readonly ConcurrentDictionary<string, CacheDataStruct> _cacheDict = new ConcurrentDictionary<string, CacheDataStruct>();

		public virtual string Get(string key)
		{
			return GetOrCreateCacheData(key).Data;
		}

		public void Set(string key, string value)
		{
			CacheData(key, value);
			Save(key, value);
		}

		public virtual async UniTask SetAsync(string key, string value)
		{
			CacheData(key, value);
			await SaveIfNeed(key);
		}

		public void DeleteAll()
		{
			_cacheDict.Clear();
			DeleteAllInternal();
		}

		protected abstract void DeleteAllInternal();
		protected abstract void Save(string key, string value);

		protected abstract UniTask SaveAsync(string key, string value);

		protected abstract SemaphoreSlim GetSemaphore(string key);

		protected virtual void SaveDataToCache(string key, string data)
		{
			var cache = GetOrCreateCacheData(key);
			cache.Data = data;
		}

		protected string LoadByName(string name)
		{
			using (var reader = new StreamReader(name))
			{
				return reader.ReadToEnd();
			}
		}

		private void CacheData(string key, string value)
		{
			var cacheData = GetOrCreateCacheData(key);
			cacheData.CurrentSaveCache = Guid.NewGuid().ToString();
			cacheData.Data = value;
		}

		private async UniTask SaveIfNeed(string key)
		{
			var semaphore = GetSemaphore(key);
			await semaphore.WaitAsync();
			var cacheData = GetOrCreateCacheData(key);
			if (!cacheData.CurrentSaveCache.Equals(cacheData.LastSaveCache))
			{
				cacheData.LastSaveCache = cacheData.CurrentSaveCache;
				await SaveAsync(key, cacheData.Data);
			}

			semaphore.Release();
		}

		private CacheDataStruct GetOrCreateCacheData(string key)
		{
			if (!_cacheDict.ContainsKey(key))
			{
				_cacheDict.TryAdd(key, new CacheDataStruct());
			}

			return _cacheDict[key];
		}
	}
}
