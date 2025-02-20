using System;
using UnityEngine;

namespace Common.Audio.Implementation.Data
{
	[Serializable]
	public class ExtendedAudioSource
	{
		public AudioSource AudioSource;

		public double StartedAtRealtimeSeconds;
	}
}