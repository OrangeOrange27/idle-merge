using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Common.Audio.Implementation.Data
{
	[Serializable]
	public class AudioConfig
	{
		[Tooltip("If multiple selected, every time need to play audio, random audio clip will be selected.")]
		public List<AudioClip> AudioClips = new();
		[NonSerialized] public List<AudioClip> OverridenAudioClips = new();
		public PlayAudioBehaviour PlayBehaviour;

		public AudioMixerGroup AudioMixerGroup;

		public bool Loop;

		[HideInInspector]
		public int Index;

		[Range(1, 16)]
		public int MaxChannelsProvided = 1;

		public bool OverrideVolume;
		
		[ShowIf("OverrideVolume")]
		[AllowNesting]
		[Range(0, 3)]
		public float OverrideVolumeValue = 1;

		/// <summary>
		/// Represents the behaviour of the audio clip when it is played.
		/// RepeatSequence: Plays the audio clips in sequence, when gets to the final clip, repeat from start.
		/// EndSequence: Plays the audio clips in sequence, when gets to the final clip, repeat last clip.
		/// Random: Plays a random audio clip from the list.
		/// </summary>
		public enum PlayAudioBehaviour
		{
			RepeatSequence,
			RepeatEndSequence,
			Random
		}
	}
}
