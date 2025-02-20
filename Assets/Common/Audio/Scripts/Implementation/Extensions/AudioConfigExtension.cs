using Common.Audio.Implementation.Data;
using UnityEngine;

namespace Common.Audio.Implementation.Extensions
{
	public static class AudioConfigExtension
	{
		public static AudioClip GetAudioClip(this AudioConfig audioConfig)
		{
			if (audioConfig.AudioClips.Count == 0)
				return null;

			var index = 0;

			switch (audioConfig.PlayBehaviour)
			{
				case AudioConfig.PlayAudioBehaviour.Random:
					index = Random.Range(0, audioConfig.AudioClips.Count);
					break;

				case AudioConfig.PlayAudioBehaviour.RepeatSequence:
					index = audioConfig.Index++ % audioConfig.AudioClips.Count;
					break;
				
				case AudioConfig.PlayAudioBehaviour.RepeatEndSequence:
					if (audioConfig.Index >= audioConfig.AudioClips.Count - 1)
					{
						index = audioConfig.AudioClips.Count - 1;
					}
					else
					{
						index = audioConfig.Index++;
					}
					break;
			}

			if (audioConfig.OverridenAudioClips.Count > index && audioConfig.OverridenAudioClips[index] != null)
			{
				return audioConfig.OverridenAudioClips[index];
			}

			return audioConfig.AudioClips[index];
		}
		
		public static void ResetIndex(this AudioConfig audioConfig)
		{
			audioConfig.Index = 0;
		}
	}
}