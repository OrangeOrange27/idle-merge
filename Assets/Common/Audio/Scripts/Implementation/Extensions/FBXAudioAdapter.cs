using ZLogger;
using Common.Audio;
using Common.Audio.Implementation.Extensions;
using Package.Logger.Abstraction;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
[AddComponentMenu("Audio/FBX Audio adapter")]
public class FBXAudioAdapter : AudioMonoBehaviourBase
{
	private static readonly ILogger Logger = LogManager.GetLogger<FBXAudioAdapter>();
	
	public void PlayAudio(Object audioConfigScriptableObject)
	{
		if (!enabled)
		{
			return;
		}

		var audioConfig = (audioConfigScriptableObject as AudioConfigScriptableObject)?.AudioConfig;

		if (audioConfig == null)
		{
			Logger.ZLogError("Please use only AudioConfigScriptableObject as Object in animation event (FBX Import settings)");
			return;
		}

		PlayAudio(audioConfig);
	}
}