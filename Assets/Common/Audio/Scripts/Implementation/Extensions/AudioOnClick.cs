using UnityEngine;
using UnityEngine.EventSystems;

namespace Common.Audio.Implementation.Extensions
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[AddComponentMenu("Audio/AudioOnClick")]
	public class AudioOnClick : AudioMonoBehaviourBase, IPointerClickHandler
	{
		[SerializeField]
		private AudioConfigScriptableObject _audioOnPointerClick;

		public void OnPointerClick(PointerEventData eventData)
		{
			if (_audioOnPointerClick != null)
				PlayAudio(_audioOnPointerClick.AudioConfig);
		}
	}
}
