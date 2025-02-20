using System.IO;
using System.Linq;
using Common.Audio.Implementation.Data;
using Tools.EditorTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Audio.Infrastructure.Editor
{
	public static class CreateAudioConfigMenuAction
	{
		private const string MenuItemName = "Assets/Create/Audio Config From Audio Clip";

		[MenuItem(MenuItemName, true, 0)]
		public static bool IsValid()
		{
			return Selection.activeObject != null && Selection.activeObject is AudioClip || Selection.objects.All(item => item is AudioClip);
		}

		[MenuItem(MenuItemName, false, 0)]
		public static void CreateAudioConfigScriptableObjectFromSelectedAudioClip()
		{
			var audioClips = Selection.objects.Cast<AudioClip>();

			foreach (var audioClip in audioClips)
			{
				var audioClipPath = $"{Path.GetDirectoryName(AssetDatabase.GetAssetPath(audioClip))}";
				var assetName = $"{audioClip.name}_AudioConfig";
				var assetDirectoryPath = $"{audioClipPath}/AudioConfigs";
				var assetPath = $"{assetDirectoryPath}/{assetName}";

				if (!Directory.Exists(assetDirectoryPath))
				{
					Directory.CreateDirectory(assetDirectoryPath);
				}

				var config = EditorUtils.LoadFirstAsset<AudioConfigScriptableObject>(assetName);

				if (config != null)
				{
					var result = EditorUtility.DisplayDialog(
						"Already present!",
						$"Config with name {assetName} already present! Do u want to create config duplicate?",
						"Yes, create copy.",
						"No, skip operation for the clip.");

					if (!result)
					{
						Selection.activeObject = config;
					}
					else
					{
						AssetDatabase.CopyAsset($"{assetPath}.asset", $"{assetPath} (Copy).asset");
						AssetDatabase.SaveAssets();
					}

					continue;
				}

				var mixer = EditorUtils.LoadFirstAsset<AudioMixer>();
				config = ScriptableObject.CreateInstance<AudioConfigScriptableObject>();

				config.AudioConfig.AudioClips.Add(audioClip);
				config.AudioConfig.AudioMixerGroup = mixer.FindMatchingGroups(AudioManagerConstants.SfxGroup).First();

				AssetDatabase.CreateAsset(config, $"{assetPath}.asset");
				AssetDatabase.SaveAssets();

				Selection.activeObject = config;
			}
		}
	}
}