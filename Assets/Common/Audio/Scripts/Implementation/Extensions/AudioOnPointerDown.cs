using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Audio.Implementation.Extensions
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[AddComponentMenu("Audio/AudioOnPointerDown")]
	public class AudioOnPointerDown : AudioMonoBehaviourBase, IPointerDownHandler
	{
		[SerializeField]
		private AudioConfigScriptableObject _audioOnPointerDown;

		public void OnPointerDown(PointerEventData eventData)
		{
			PlayAudio(_audioOnPointerDown.AudioConfig);
		}
	}
}
