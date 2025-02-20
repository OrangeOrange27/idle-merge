using UnityEngine;
using UnityEngine.Audio;

namespace Common.Audio.Implementation
{
	[CreateAssetMenu(fileName = "AudioMixerContainer", menuName = "ScriptableObjects/Audio Mixer Container", order = 3)]
	public class AudioMixerContainer : ScriptableObject
	{
		[SerializeField]
		private AudioMixer _audioMixer;

		public AudioMixer AudioMixer => _audioMixer;
	}
}