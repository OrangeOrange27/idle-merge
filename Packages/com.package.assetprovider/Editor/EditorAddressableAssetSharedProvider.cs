using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace Common.Tests.EditorAssetsCache
{
	public sealed class EditorAddressableAssetSharedProvider
	{
		private const int DefaultAssetEntriesCapacity = 1000;

		public static EditorAddressableAssetSharedProvider Instance { get; }
		static EditorAddressableAssetSharedProvider() => Instance = new EditorAddressableAssetSharedProvider();

		private readonly Dictionary<string, UnityEngine.Object> _addressableAssetsCache = new Dictionary<string, UnityEngine.Object>();
		private readonly Dictionary<string, AddressableAssetEntry> _addressableEntriesByAddress = new Dictionary<string, AddressableAssetEntry>();

		private readonly Dictionary<string, UnityEngine.Object> _assetDatabaseAssetsCache = new Dictionary<string, UnityEngine.Object>();
		private readonly Dictionary<string, Dictionary<string, List<string>>> _assetDatabaseSearchGuids = new Dictionary<string, Dictionary<string, List<string>>>();
		private readonly Dictionary<string, string> _assetDatabaseGuidsToAssetPaths = new Dictionary<string, string>();
		private readonly Dictionary<string, List<string>> _assetDatabaseKeyToPathCache = new Dictionary<string, List<string>>();

		private EditorAddressableAssetSharedProvider()
		{
		}

		public void Initialize()
		{
			CleanUp();
			LoadAddressableAssets();
		}

		public void CleanUp()
		{
			_addressableAssetsCache.Clear();
			_addressableEntriesByAddress.Clear();
			_assetDatabaseSearchGuids.Clear();
			_assetDatabaseGuidsToAssetPaths.Clear();
		}

		public T LoadAssetAsync<T>(string key) where T : UnityEngine.Object
		{
			UnityEngine.Object asset = null;

			if (TryLoadAssetFromCache(key, ref asset))
				return (T)asset;

			if (TryLoadAssetFromAddressables<T>(key, ref asset))
				return (T)asset;

			return LoadAssetFromDatabase<T>(key);
		}

		private bool TryLoadAssetFromCache(string key, ref UnityEngine.Object asset)
		{
			if (_addressableAssetsCache.TryGetValue(key, out var cachedAsset))
			{
				asset = cachedAsset;
				return true;
			}

			return false;
		}

		private bool TryLoadAssetFromAddressables<T>(string key, ref UnityEngine.Object asset) where T : UnityEngine.Object
		{
			if (_addressableEntriesByAddress != null && _addressableEntriesByAddress.TryGetValue(key, out var entry))
			{
				asset = AssetDatabase.LoadAssetAtPath<T>(entry.AssetPath);
				_addressableAssetsCache[key] = asset;

				return true;
			}

			return false;
		}

		private T LoadAssetFromDatabase<T>(string key) where T : UnityEngine.Object
		{
			var assetsPaths = LoadAssetsPathsFromAssetDatabase<T>(key)
				.Where(a => GetAssetNameFromPath(a) == key || IsMultipleSprite(a))
				.ToList();

			var results = new List<T>(assetsPaths.Count);

			foreach (var assetsPath in assetsPaths)
			{
				if (!_assetDatabaseAssetsCache.TryGetValue(assetsPath, out var cachedAsset))
				{
					cachedAsset = AssetDatabase.LoadAssetAtPath<T>(assetsPath);
					_assetDatabaseAssetsCache[assetsPath] = cachedAsset;
				}

				if (cachedAsset.name != key)
					continue;

				results.Add((T)cachedAsset);
			}

			bool IsDoubledSprite()
			{
				return results.FirstOrDefault() is Sprite && results.Count == 2;
			}

			if (results.Count > 1 && !IsDoubledSprite())
			{
				throw new Exception($"Expected only 1 asset (type={typeof(T).Name} name='{key}') but found {results.Count} assets");
			}

			return results.FirstOrDefault();
		}

		public string GetAssetPath<T>(string key) where T : UnityEngine.Object
		{
			if (_addressableEntriesByAddress.TryGetValue(key, out var asset))
				return asset.AssetPath;

			return GetAssetDatabaseAssetPath<T>(key);
		}

		private string GetAssetDatabaseAssetPath<T>(string name = "") where T : UnityEngine.Object
		{
			var assetPaths = LoadAssetsPathsFromAssetDatabase<T>(name);

			return assetPaths.LastOrDefault();
		}

		private List<string> LoadAssetsPathsFromAssetDatabase<T>(string key)
		{
			if (!_assetDatabaseKeyToPathCache.TryGetValue(key, out var assetPaths))
			{
				var assetGuids = LoadAssetsGuidsFromAssetDatabase<T>(key);
				assetPaths = new List<string>(assetGuids.Count);

				foreach (var assetGuid in assetGuids)
				{
					if (!_assetDatabaseGuidsToAssetPaths.TryGetValue(assetGuid, out var assetPath))
					{
						assetPath = AssetDatabase.GUIDToAssetPath(assetGuid);
						_assetDatabaseGuidsToAssetPaths[assetGuid] = assetPath;
					}

					assetPaths.Add(assetPath);
				}

				_assetDatabaseKeyToPathCache[key] = assetPaths;
			}

			return assetPaths;
		}

		private List<string> LoadAssetsGuidsFromAssetDatabase<T>(string key)
		{
			var assetType = typeof(T);
			var typeName = assetType.Name;

			if (!_assetDatabaseSearchGuids.TryGetValue(typeName, out var keys))
			{
				keys = new Dictionary<string, List<string>>
				{
					{ key, null }
				};

				_assetDatabaseSearchGuids[typeName] = keys;
			}

			if (!keys.TryGetValue(key, out var assetGuids) || assetGuids == null)
			{
				assetGuids = AssetDatabase
					.FindAssets($"t:{assetType.Name} {key}")
					.ToList();

				keys[key] = assetGuids;
			}

			return assetGuids;
		}

		private void LoadAddressableAssets()
		{
			var entries = new List<AddressableAssetEntry>(DefaultAssetEntriesCapacity);

			AddressableAssetSettingsDefaultObject.Settings.GetAllAssets(entries, true);

			foreach (var entry in entries)
			{
				_addressableEntriesByAddress[entry.address] = entry;
			}
		}

		private string GetAssetNameFromPath(string path)
		{
			return Path.GetFileNameWithoutExtension(path);
		}

		private bool IsMultipleSprite(string path)
		{
			var extension = Path.GetExtension(path);
			return extension == ".png";
		}
	}
}