using UnityEngine;

namespace Common.Audio.Implementation.Extensions
{
	[ExecuteAlways]
	[AddComponentMenu("Audio/Audio")]
	public class Audio : AudioMonoBehaviourBase
	{
		[SerializeField]
		private AudioConfigScriptableObject _audioConfig;

		[SerializeField]
		private bool _playOnEnable;

		[SerializeField, NaughtyAttributes.ShowIf("_playOnEnable")]
		private bool _stopOnDisable;

		private void OnEnable()
		{
			if (_playOnEnable)
			{
				PlayAudio(_audioConfig.AudioConfig);
			}
		}

		public void Play(int audioIndex = 0)
		{
			_audioConfig.AudioConfig.Index = audioIndex;
			PlayAudio(_audioConfig.AudioConfig);
		}

		private void OnDisable()
		{
			if (_stopOnDisable)
			{
				StopAudio();
			}
		}
	}
}
