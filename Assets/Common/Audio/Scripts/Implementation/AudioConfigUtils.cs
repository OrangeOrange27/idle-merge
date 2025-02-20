using System;
using System.Threading;
using Common.Audio.Implementation.Data;
using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Package.AssetProvider.Infrastructure;

namespace Common.Audio.Implementation
{
	public class AudioConfigUtils
	{
		public static async UniTask<AudioConfig> LoadAudioConfigAsync(IAssetProvider assetProvider, string configName,
			ILogger logger, CancellationToken token)
		{
			try
			{
				var buildSoundSO = await assetProvider.LoadAsync<AudioConfigScriptableObject>(configName, token);

				return buildSoundSO.AudioConfig;
			}
			catch (Exception e)
			{
				logger.LogError($"Failed to load {configName}", e);
			}

			return null;
		}
	}
}