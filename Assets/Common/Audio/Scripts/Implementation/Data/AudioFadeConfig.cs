using System;

namespace Common.Audio.Implementation.Data
{
	[Serializable]
	public struct AudioFadeConfig
	{
		public static AudioFadeConfig Default;

		public float FadeInDuration;
		public float FadeOutDuration;

		public bool Equals(AudioFadeConfig other)
		{
			return FadeInDuration.Equals(other.FadeInDuration) && FadeOutDuration.Equals(other.FadeOutDuration);
		}
	}
}
