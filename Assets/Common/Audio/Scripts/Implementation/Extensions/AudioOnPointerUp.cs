using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Audio.Implementation.Extensions
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[AddComponentMenu("Audio/AudioOnPointerUp")]
	public class AudioOnPointerUp : AudioMonoBehaviourBase, IPointerUpHandler
	{
		[SerializeField]
		private AudioConfigScriptableObject _audioOnPointerUp;

		public void OnPointerUp(PointerEventData eventData)
		{
			PlayAudio(_audioOnPointerUp.AudioConfig);
		}
	}
}
