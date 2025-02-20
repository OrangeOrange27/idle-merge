using System;
using System.Threading;
using Common.Audio.Implementation.Data;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Package.AssetProvider.Infrastructure;

namespace Common.Audio.Infrastructure
{
	public interface IAudioManager
	{
		event Action<bool> OnAudioConfigurationChanged;

		UniTask Initialize(CancellationToken token);
		void SetSfxVolumeActive(bool isActive);
		void SetMusicVolumeActive(bool isActive);
		UniTask SetMusicVolumeActiveAsync(bool isActive, CancellationToken cancellationToken);
		UniTask SetMusicVolumeAsync(float to, CancellationToken cancellationToken, Ease ease = Ease.InOutCubic);
		UniTask SetDefaultMusicVolumeAsync(CancellationToken cancellationToken, Ease ease = Ease.InOutCubic);

		UniTask Play(string soundName, IAssetProvider assetProvider, CancellationToken token = default);
		UniTask PlayAudio(
			AudioConfig audioConfig,
			CancellationToken token,
			IConvertible channel = default,
			bool forceStopPlayingAudio = false,
			AudioFadeConfig audioFadeConfig = default,
			bool? loopOverride = default,
			float playbackTime = default);

		float GetPlaybackTime(IConvertible channel);
		UniTask FadeOutAndStopChannel(IConvertible channel, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken);
		void StopFirstAudioInChannel(IConvertible channel);
		UniTask FadeChannel(bool isActive, IConvertible channel, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken);
		UniTask FadeToVolume(bool isActive, IConvertible channel, float volume, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken);
	}
}