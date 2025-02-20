using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Tools.EditorTools
{
	public static class EditorUtils
	{
		public static List<T> LoadAssets<T>(string name = "") where T : UnityEngine.Object
		{
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<T>)
				.ToList();
		}

		public static string GetAssetPath<T>(string name = "") where T : UnityEngine.Object
		{
			var dict = new Dictionary<string, string>();
			foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}"))
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var key = AssetDatabase.LoadAssetAtPath<T>(assetPath).name;

				if (dict.ContainsKey(key))
				{
					dict[key] = assetPath;
				}
				else
				{
					dict.Add(key, assetPath);
				}
			}

			if (dict.TryGetValue(name, out var path))
			{
				return path;
			}

			return string.Empty;
		}

		public static List<T> LoadAssetsByPath<T>(params string[] paths) where T : UnityEngine.Object
		{
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} ", paths)
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<T>)
				.ToList();
		}

		public static Dictionary<string, T> LoadAssetsToDictionary<T>(string name = "") where T : UnityEngine.Object
		{
			return AssetDatabase.FindAssets($"t:{typeof(T).Name} {name}")
				.Select(AssetDatabase.GUIDToAssetPath)
				.Select(AssetDatabase.LoadAssetAtPath<T>)
				.ToDictionary(x => x.name, StringComparer.InvariantCultureIgnoreCase);
		}

		public static T LoadFirstAsset<T>(string name = "") where T : UnityEngine.Object
		{
			return LoadAssets<T>(name).FirstOrDefault();
		}

		public static T LoadSingleAsset<T>(string name = "") where T : UnityEngine.Object
		{
			var results = LoadAssets<T>(name).Where(result => result.name == name).ToList();

			if (results.Count > 1 && !IsDoubledSprite())
			{
				throw new Exception($"Expected only 1 asset (type={typeof(T).Name} name='{name}') but found {results.Count} assets");
			}

			return results.FirstOrDefault();

			bool IsDoubledSprite()
			{
				return results.FirstOrDefault() is Sprite && results.Count == 2;
			}
		}

		public static T LoadExistingAsset<T>(string name = "") where T : UnityEngine.Object
		{
			var results = LoadAssets<T>(name);

			if (results == null || results.Count == 0)
			{
				throw new Exception($"Asset (type={typeof(T).Name} name='{name}') not found!");
			}

			if (results.Count > 1)
			{
				throw new Exception($"Expected only 1 asset (type={typeof(T).Name} name='{name}') but found {results.Count} assets");
			}

			return results.FirstOrDefault();
		}

		public static IEnumerable<string> EnumerateAssets(string type, string folder)
		{
			var allScenes = AssetDatabase.FindAssets($"t:{type}", new[] { folder });

			return allScenes.Select(x => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(x))).OrderBy(x => x).ToArray();
		}

		public static IEnumerable<string> EnumerateAssets<T>(string folder) where T : UnityEngine.Object
		{
			var allScenes = AssetDatabase.FindAssets($"t:{typeof(T).FullName}", new[] { folder });

			return allScenes.Select(x => Path.GetFileNameWithoutExtension(AssetDatabase.GUIDToAssetPath(x))).OrderBy(x => x).ToArray();
		}

		public static T CreateScriptableObject<T>(string path, string name) where T : ScriptableObject
		{
			var asset = ScriptableObject.CreateInstance<T>();

			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}

			AssetDatabase.CreateAsset(asset, Path.Combine(path, $"{name}.asset"));

			return asset;
		}

		public static void DeleteAsset(UnityEngine.Object asset)
		{
			if (!asset)
			{
				return;
			}

			if (!AssetDatabase.TryGetGUIDAndLocalFileIdentifier(asset, out var guid, out long _))
			{
				throw new Exception($"Cannot get GUID for asset: '{asset.name}'");
			}

			var path = AssetDatabase.GUIDToAssetPath(guid);
			AssetDatabase.DeleteAsset(path);
		}
	}
}
