using Common.Audio.Implementation.Data;
using UnityEngine;

namespace Common.Audio
{
	[CreateAssetMenu(fileName = "AudioConfig", menuName = "XternityConfig/AudioConfig")]
	public class AudioConfigScriptableObject : ScriptableObject
	{
		public AudioConfig AudioConfig = new();
	}
}
