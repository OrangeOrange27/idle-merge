using Cysharp.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;

namespace Features.SplashScreen
{
    public class GameLoadingManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameLoadingManager>();

        public bool IsGameDataInitComplete => true;
        public bool IsSignInDone => true;

        public async UniTask LoadGame()
        {
            
        }
    }
}