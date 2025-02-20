using System.Collections.Generic;
using Common.Audio.Implementation.Data;
using UnityEngine;

namespace Common.Audio.Implementation.Extensions
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[AddComponentMenu("Audio/Audio Animator")]
	public class AudioAnimator : AudioMonoBehaviourBase
	{
		[SerializeField]
		private List<AudioConfigScriptableObject> _audioConfigs = new List<AudioConfigScriptableObject>();

		/// <summary>
		/// The value to change through Animator or Animation component. The source of playing the specific audio
		/// </summary>
		public int AudioConfigIndex = -1;
		public AudioFadeConfig _audioFadeConfig;

		private int _currentAudioConfigIndex = -1;
		private string _audioFadeConfigHash;

		public AudioConfigScriptableObject CurrentAudio => IsIndexValid ? _audioConfigs[AudioConfigIndex] : null;
		private bool IsIndexValid => AudioConfigIndex >= 0 && AudioConfigIndex < _audioConfigs.Count;

		private void OnDidApplyAnimationProperties()
		{
			if (!IsUniqueChannelPerAudio || !IsIndexValid)
			{
				StopAudio();
			}

			var shouldPlay = AudioConfigIndex != _currentAudioConfigIndex && IsIndexValid;

			_currentAudioConfigIndex = AudioConfigIndex;

			if (!shouldPlay)
			{
				return;
			}

			PlayAudio(_audioConfigs[AudioConfigIndex].AudioConfig, _audioFadeConfig);
		}
	}
}