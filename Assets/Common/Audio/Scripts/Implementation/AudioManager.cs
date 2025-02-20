using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Common.Audio.Implementation.Data;
using Common.Audio.Implementation.Extensions;
using Common.Audio.Infrastructure;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Package.AssetProvider.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using UnityEngine.Audio;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Audio.Implementation
{
	public class AudioManager : IAudioManager, IDisposable
	{
		private static readonly ILogger Logger = LogManager.GetLogger<AudioManager>();
		
		public event Action<bool> OnAudioConfigurationChanged;

		private const string AudioMixerContainerKey = "AudioMixerContainer";
		private const string FreeAudioSourceName = "Free Audio Source";

		private const int AudioSourcesCountOnStart = 5;
		private const int MaxSupportedAudioSourcesCount = 32;
		private const float MusicFadeDuration = 1f;

		private readonly IAssetProvider _assetProvider;
		private readonly Stack<ExtendedAudioSource> _freeAudioSourcesPool = new Stack<ExtendedAudioSource>();
		private readonly Dictionary<IConvertible, List<ExtendedAudioSource>> _activeChannels = new Dictionary<IConvertible, List<ExtendedAudioSource>>();

		private int _audioSourcesCount;
		private AudioMixer _audioMixer;
		private Transform _audioSourcesParent;
		private int _lastId;
		private bool _isMusicActive;
		private bool _isSfxActive;

		/// <summary>
		/// Use only with MonoBehaviour where we do not have Diy, in other situations resolve Type from diy
		/// </summary>
		public static AudioManager Instance { get; private set; }

		public AudioManager(IAssetProvider assetProvider)
		{
			_assetProvider = assetProvider;
			Instance = this;
		}

		public async UniTask Initialize(CancellationToken token)
		{
			_audioSourcesParent = new GameObject("AudioSources").transform;

			for (var i = 0; i < AudioSourcesCountOnStart; i++)
			{
				_freeAudioSourcesPool.Push(CreateNewAudioSource());
			}

			var container = await _assetProvider.LoadAsync<AudioMixerContainer>(AudioMixerContainerKey, token);

			if (container.AudioMixer == null)
			{
				throw new NullReferenceException("AudioMixer has not been assigned in AudioMixerContainer.");
			}
			_audioMixer = container.AudioMixer;

			AudioSettings.OnAudioConfigurationChanged += OnAudioSettingsConfigurationChanged;
		}

		public void Dispose()
		{
			foreach (var activeChannel in _activeChannels.Keys.ToList())
			{
				CleanUpAllAudioSourcesInChannel(activeChannel);
			}

			foreach (var freeAudioSource in _freeAudioSourcesPool)
			{
				UnityEngine.Object.Destroy(freeAudioSource.AudioSource.gameObject);
			}

			AudioSettings.OnAudioConfigurationChanged -= OnAudioSettingsConfigurationChanged;

			Instance = null;
		}

		public void SetSfxVolumeActive(bool isActive)
		{
			_isSfxActive = isActive;

			SetSfxVolume(isActive ? AudioManagerConstants.EnabledGroupVolume : AudioManagerConstants.DisabledGroupVolume);
		}

		public void SetMusicVolumeActive(bool isActive)
		{
			_isMusicActive = isActive;

			SetMusicVolume(isActive ? AudioManagerConstants.EnabledGroupVolume : AudioManagerConstants.DisabledGroupVolume);
		}

		public async UniTask SetMusicVolumeActiveAsync(bool isActive, CancellationToken cancellationToken)
		{
			_isMusicActive = isActive;

			var from = isActive ? AudioManagerConstants.DisabledGroupVolume : AudioManagerConstants.EnabledGroupVolume;
			var to = isActive ? AudioManagerConstants.EnabledGroupVolume : AudioManagerConstants.DisabledGroupVolume;
			var ease = isActive ? Ease.OutSine : Ease.InSine;

			await SetMusicVolumeAsyncInner(from, to, ease, cancellationToken);
		}

		public async UniTask SetMusicVolumeAsync(float to, CancellationToken cancellationToken, Ease ease = Ease.InOutCubic)
		{
			if (!_isMusicActive)
			{
				return;
			}

			_audioMixer.GetFloat(AudioManagerConstants.MusicGroup, out var from);

			await SetMusicVolumeAsyncInner(from, to, ease, cancellationToken);
		}

		public async UniTask SetDefaultMusicVolumeAsync(CancellationToken cancellationToken, Ease ease = Ease.InOutCubic)
		{
			if (!_isMusicActive)
			{
				return;
			}

			_audioMixer.GetFloat(AudioManagerConstants.MusicGroup, out var from);

			await SetMusicVolumeAsyncInner(from, AudioManagerConstants.EnabledGroupVolume, ease, cancellationToken);
		}
		
		public async UniTask Play(string soundName, IAssetProvider assetProvider, CancellationToken token = default)
		{
			var audioConfig = await assetProvider.LoadAsync<AudioConfigScriptableObject>(soundName, token);

			await PlayAudio(audioConfig.AudioConfig, token);
		}

		public async UniTask PlayAudio(
			AudioConfig audioConfig,
			CancellationToken token,
			IConvertible channel = default,
			bool forceStopPlayingAudio = false,
			AudioFadeConfig audioFadeConfig = default,
			bool? loopOverride = default,
			float playbackTime = default)
		{
			token.ThrowIfCancellationRequested();

			if (audioConfig.AudioClips.Count == 0)
			{
				Logger.ZLogWarning("There are no audio clip referenced in config. Will stop the {0} chanel.", channel);

				return;
			}

			if (channel == null)
				channel = _lastId++;

			var audioSource = GetAudioSource(audioConfig.MaxChannelsProvided, channel, forceStopPlayingAudio);

			if (audioSource == null)
			{
				return;
			}

			try
			{
				AddAudioSourceToChannel(channel, audioSource);

				var source = audioSource.AudioSource;
				source.clip = audioConfig.GetAudioClip();
				source.outputAudioMixerGroup = audioConfig.AudioMixerGroup;
				source.loop = loopOverride ?? audioConfig.Loop;
				if (audioConfig.OverrideVolume)
				{
					source.volume *= audioConfig.OverrideVolumeValue;
				}
				
				source.name = channel.ToString(CultureInfo.CurrentCulture);
				source.time = playbackTime <= source.clip.length ? playbackTime : 0f;
				source.Play();
				audioSource.StartedAtRealtimeSeconds = Time.realtimeSinceStartup;

				await TryFadeIn(source, audioFadeConfig, token);

				await UniTask.WaitWhile(
					() => source != null && source.isPlaying,
					cancellationToken: token,
					timing: PlayerLoopTiming.LastPostLateUpdate);
			}
			catch (OperationCanceledException)
			{
				await FadeOutAndStopChannel(channel, audioFadeConfig, CancellationToken.None);
			}
			finally
			{
				CleanUpAudioSourceInChannel(channel, audioSource);
			}
		}

		public float GetPlaybackTime(IConvertible channel)
		{
			if (!_activeChannels.ContainsKey(channel))
			{
				return 0;
			}

			var audioInfo = _activeChannels[channel];
			var audioSource = audioInfo.First();

			return audioSource.AudioSource.time;
		}

		public async UniTask FadeOutAndStopChannel(IConvertible channel, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken)
		{
			await FadeChannel(false, channel, audioFadeConfig, cancellationToken);

			StopFirstAudioInChannel(channel);
		}

		public void StopFirstAudioInChannel(IConvertible channel)
		{
			if (channel == null || !_activeChannels.ContainsKey(channel))
			{
				return;
			}

			var channelAudioSources = _activeChannels[channel];
			var audioSource = channelAudioSources.FirstOrDefault();

			if (audioSource != null)
			{
				audioSource.AudioSource.Stop();
			}
		}

		public async UniTask FadeChannel(bool isActive, IConvertible channel, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken)
		{
			if (channel == null || !_activeChannels.ContainsKey(channel))
			{
				return;
			}

			var channelAudioSources = _activeChannels[channel];
			var audioSource = channelAudioSources.First();

			if (!AudioFadeConfig.Default.Equals(audioFadeConfig) && audioSource.AudioSource.isPlaying)
			{
				var targetVolume = isActive ? 1 : 0;
				var fadeDuration = isActive ? audioFadeConfig.FadeInDuration : audioFadeConfig.FadeOutDuration;
				
				await audioSource.AudioSource.DOFade(targetVolume, fadeDuration).SetEase(Ease.Linear).WithCancellation(cancellationToken);
			}
		}

		public async UniTask FadeToVolume(bool isActive, IConvertible channel, float volume, AudioFadeConfig audioFadeConfig, CancellationToken cancellationToken)
		{
			if (channel == null || !_activeChannels.ContainsKey(channel)) return;

			var audioInfo = _activeChannels[channel];
			var audioSource = audioInfo.First();

			if (!AudioFadeConfig.Default.Equals(audioFadeConfig) && audioSource.AudioSource.isPlaying)
			{
				var targetVolume = volume;
				var fadeDuration = isActive ? audioFadeConfig.FadeInDuration : audioFadeConfig.FadeOutDuration;

				await audioSource.AudioSource.DOFade(targetVolume, fadeDuration).SetEase(Ease.Linear).WithCancellation(cancellationToken);
			}
		}

		private void SetMusicVolume(float volume) => _audioMixer.SetFloat(AudioManagerConstants.MusicGroup, volume);

		private void SetSfxVolume(float volume) => _audioMixer.SetFloat(AudioManagerConstants.SfxGroup, volume);

		private async UniTask SetMusicVolumeAsyncInner(float from, float to, Ease ease, CancellationToken cancellationToken)
		{
			await DOVirtual.Float(from, to, MusicFadeDuration, SetMusicVolume)
				.SetEase(ease)
				.ToUniTask(TweenCancelBehaviour.KillWithCompleteCallbackAndCancelAwait, cancellationToken);
		}

		private ExtendedAudioSource GetAudioSource(int maxChannelsProvided, IConvertible channel, bool isForceStopSound)
		{
			ExtendedAudioSource audioSource = null;

			if (_activeChannels.ContainsKey(channel))
			{
				if (_activeChannels[channel].Count < maxChannelsProvided)
				{
					audioSource = GetFreeAudioSource();
				}

				if (audioSource == null && isForceStopSound && _activeChannels[channel].Count > 0)
				{
					StopFirstAudioInChannel(channel);

					audioSource = GetFreeAudioSource();
				}

				return audioSource;
			}

			return GetFreeAudioSource();
		}

		private void AddAudioSourceToChannel(IConvertible channel, ExtendedAudioSource audioSource)
		{
			if (!_activeChannels.ContainsKey(channel))
			{
				var channelAudioSources = new List<ExtendedAudioSource>();

				_activeChannels.Add(channel, channelAudioSources);
			}

			_activeChannels[channel].Add(audioSource);
		}

		private void CleanUpAudioSourceInChannel(IConvertible channel, ExtendedAudioSource audioSource)
		{
			if (!_activeChannels.ContainsKey(channel))
			{
				return;
			}

			CleanUpAudioSource(audioSource);
			var channelAudioSources = _activeChannels[channel];

			channelAudioSources.Remove(audioSource);

			if (channelAudioSources.Count == 0)
			{
				_activeChannels.Remove(channel);
			}
		}

		private void CleanUpAudioSource(ExtendedAudioSource audioSource)
		{
			var source = audioSource.AudioSource;

			if (source != null)
			{
				source.clip = null;
				source.volume = 1.0f;
				source.loop = false;
				source.outputAudioMixerGroup = null;
				source.name = FreeAudioSourceName;
				audioSource.StartedAtRealtimeSeconds = 0;
				_freeAudioSourcesPool.Push(audioSource);
			}
		}

		private void CleanUpAllAudioSourcesInChannel(IConvertible channel)
		{
			if (!_activeChannels.ContainsKey(channel))
			{
				return;
			}

			var channelAudioSources = _activeChannels[channel].ToList();

			foreach (var audioSource in channelAudioSources)
			{
				CleanUpAudioSourceInChannel(channel, audioSource);
			}
		}

		private async UniTask TryFadeIn(AudioSource audioSource, AudioFadeConfig audioFadeConfig, CancellationToken token)
		{
			token.ThrowIfCancellationRequested();

			if (!AudioFadeConfig.Default.Equals(audioFadeConfig))
			{
				audioSource.volume = 0;
				await audioSource.DOFade(1, audioFadeConfig.FadeInDuration).SetEase(Ease.Linear).WithCancellation(token);
			}
		}

		private ExtendedAudioSource CreateNewAudioSource()
		{
			if (_audioSourcesCount >= MaxSupportedAudioSourcesCount)
			{
				var logMessageBuilder = new StringBuilder(2500);

				var currentTimeSeconds = Time.realtimeSinceStartup;

				logMessageBuilder.AppendFormat("Before clean-up active channels {0}\n", _activeChannels.Count);
				BuildErrorLog(logMessageBuilder, currentTimeSeconds);

				CleanAllChannelsFromDeadAudio(currentTimeSeconds);

				logMessageBuilder.AppendFormat("After clean-up active channels {0}\n", _activeChannels.Count);
				BuildErrorLog(logMessageBuilder, currentTimeSeconds);


				Logger.ZLogError("Audio channels overflow. We support only {0} audio sources. ChannelsInfo {1}", MaxSupportedAudioSourcesCount,
					logMessageBuilder.ToString());

				return null;
			}

			var go = new GameObject(FreeAudioSourceName);
			go.transform.SetParent(_audioSourcesParent);

			_audioSourcesCount++;
			var extended = new ExtendedAudioSource();
			extended.AudioSource = go.AddComponent<AudioSource>();

			return extended;
		}

		private void CleanAllChannelsFromDeadAudio(double currentTimeSeconds)
		{
			var allKeys = _activeChannels.Keys.ToList();

			foreach (var channelKey in allKeys)
			{
				var channelList = _activeChannels[channelKey];
				var audioSourcesNotPlaying = channelList
					.Where(source => source != null &&
									 source.AudioSource != null &&
									 (!source.AudioSource.isPlaying ||
									  (!source.AudioSource.loop &&
									   (currentTimeSeconds - source.StartedAtRealtimeSeconds) > source.AudioSource.clip.length)))
					.ToList();

				foreach (var audioSource in audioSourcesNotPlaying)
				{
					CleanUpAudioSourceInChannel(channelKey, audioSource);
				}
			}
		}

		private void BuildErrorLog(StringBuilder logMessageBuilder, double currentTimeInSec)
		{
			foreach (var channelInfo in _activeChannels)
			{
				var audioSourcesNames = string.Join(", ",
					channelInfo.Value
						.Where(source => source != null && source.AudioSource != null && source.AudioSource.clip != null)
						.Select(
							source =>
								$"{source.AudioSource.clip.name} : duration {currentTimeInSec - source.StartedAtRealtimeSeconds} isPLaying {source.AudioSource.isPlaying} isLoop {source.AudioSource.loop}"));

				var audioWithNull = string.Join(", ",
					channelInfo.Value
						.Where(source => source == null || source.AudioSource == null)
						.Select(
							source =>
							{
								var durationInSeconds = source == null ? 0 : currentTimeInSec - source.StartedAtRealtimeSeconds;

								return $"has null audio source duration: {durationInSeconds}";
							}));

				logMessageBuilder.AppendFormat("Channel {0} contains {1} audio sources : {2} {3}\n", channelInfo.Key, channelInfo.Value.Count, audioSourcesNames, audioWithNull);
			}
		}

		private ExtendedAudioSource GetFreeAudioSource()
		{
			return _freeAudioSourcesPool.Count == 0 ? CreateNewAudioSource() : _freeAudioSourcesPool.Pop();
		}

		private void OnAudioSettingsConfigurationChanged(bool deviceWasChanged)
		{
			if (deviceWasChanged)
			{
				var audioConfiguration = AudioSettings.GetConfiguration();
				AudioSettings.Reset(audioConfiguration);
			}

			SetSfxVolumeActive(_isSfxActive);
			SetMusicVolumeActive(_isMusicActive);

			OnAudioConfigurationChanged?.Invoke(deviceWasChanged);
		}
	}
}