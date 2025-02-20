using System.Collections.Generic;
using System.Threading;
using Common.Audio.Implementation.Data;
using Cysharp.Threading.Tasks;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Audio.Implementation.Extensions
{
	public abstract class AudioMonoBehaviourBase : MonoBehaviour
	{
		private static readonly ILogger Logger = LogManager.GetLogger<AudioMonoBehaviourBase>();
		
		private const string EditorAudioSourceName = "EditorAudioSource";
		private static AudioSource _editorAudioSource;

		[SerializeField]
		protected bool _stopAudioOnDestroy = false;
		[SerializeField]
		protected bool _isUniqueChannelPerAudio = false;

		private static AudioSource EditorAudioSource
		{
			get
			{
				if (_editorAudioSource == null)
				{
					var obj = GameObject.Find(EditorAudioSourceName);

					if (obj == null)
					{
						obj = new GameObject(EditorAudioSourceName);

						#if UNITY_EDITOR
						if (UnityEditor.SceneManagement.EditorSceneManager.IsPreviewSceneObject(obj))
						{
							UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(obj, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
						}
						#endif
						_editorAudioSource = obj.AddComponent<AudioSource>();
						EditorAudioSource.playOnAwake = false;
						obj.hideFlags = HideFlags.HideAndDontSave;
						obj.tag = "EditorOnly";
					}
					else
					{
						_editorAudioSource = obj.GetComponent<AudioSource>();
					}
				}

				return _editorAudioSource;
			}
		}

		protected bool IsUniqueChannelPerAudio => _isUniqueChannelPerAudio;
		protected bool IsInitialized => _channelPrefix != null;

		private CancellationTokenSource _cancellationTokenSource;
		private HashSet<string> _channels;

		private string _channelPrefix;

		protected virtual void Awake()
		{
			_cancellationTokenSource = new CancellationTokenSource();
			_channels = new HashSet<string>();
			_channelPrefix = GetInstanceID().ToString();
		}

		protected virtual void OnDestroy()
		{
			if (_stopAudioOnDestroy)
			{
				if (_cancellationTokenSource != null)
				{
					_cancellationTokenSource.Cancel();
					_cancellationTokenSource.Dispose();
				}
			}
		}

		protected void PlayAudio(
			AudioConfig audioConfig,
			AudioFadeConfig audioFadeConfig = default,
			bool? loopOverride = default)
		{
			if (!IsInitialized)
			{
				return;
			}

			if (Application.isPlaying)
			{
				if (AudioManager.Instance == null)
				{
					Logger.ZLogError("AudioManager instance not found, check if you use UIEnviromentSceneContext if you work outside of main scene");

					return;
				}

				var channel = _isUniqueChannelPerAudio
					? GetAudioConfigChannelName(audioConfig)
					: _channelPrefix;

				_channels.Add(channel);

				AudioManager.Instance.PlayAudio(audioConfig, _cancellationTokenSource.Token, channel, false, audioFadeConfig, loopOverride).Forget();
			}
			else
			{
				EditorAudioSource.clip = audioConfig.GetAudioClip();
				EditorAudioSource.outputAudioMixerGroup = audioConfig.AudioMixerGroup;
				EditorAudioSource.loop = audioConfig.Loop;
				EditorAudioSource.Play();
			}
		}

		public void StopAudio()
		{
			if (Application.isPlaying)
			{
				foreach (var channel in _channels)
				{
					AudioManager.Instance.StopFirstAudioInChannel(channel);
				}

				_channels.Clear();
			}
			else
			{
				EditorAudioSource.Stop();
			}
		}

		private string GetAudioConfigChannelName(AudioConfig audioConfig)
		{
			return $"{_channelPrefix}_{audioConfig.GetHashCode()}";
		}
	}
}