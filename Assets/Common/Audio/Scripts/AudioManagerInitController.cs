using System.Threading;
using Common.Audio.Infrastructure;
using Common.EntryPoint.Initialize;
using Cysharp.Threading.Tasks;

namespace Common.Audio
{
    public class AudioManagerInitController : IBeforeAuthInitialize
    {
        private readonly IAudioManager _audioManager;

        public AudioManagerInitController(IAudioManager audioManager)
        {
            _audioManager = audioManager;
        }

        public async UniTask InitializeBeforeAuth()
        {
            await _audioManager.Initialize(CancellationToken.None);
        }
    }
}
