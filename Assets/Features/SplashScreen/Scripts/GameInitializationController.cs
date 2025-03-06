using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Authentication.Providers;
using Common.EntryPoint.Initialize;
using Common.PlayerData;
using Cysharp.Threading.Tasks;

namespace Features.SplashScreen
{
    public class GameInitializationController : IBeforeAuthInitialize
    {
        private enum LoadingModule
        {
            PlayerData,
            GameData,
            //todo: add more modules here (firebase, etc)
        }
        
        private readonly SplashSceneView _splashSceneView;
        private readonly IPlayerDataService _playerDataService;

        private static readonly Dictionary<LoadingModule, float> LoadingProgress = new()
        {
            { LoadingModule.PlayerData, 0.5f },
            { LoadingModule.GameData, 0.5f }
        };

        private bool _isGameDataInitComplete;
        private bool IsSignInDone => _playerDataService.IsSignedIn;

        public GameInitializationController(SplashSceneView splashSceneView, IPlayerDataService playerDataService)
        {
            _splashSceneView = splashSceneView; 
            _playerDataService = playerDataService;
        }

        public async UniTask InitializeBeforeAuth()
        {
            var loadedGameTask = LoadGame();

            await LoadComponentAsync(() => IsSignInDone, progress => LoadingProgress[LoadingModule.PlayerData] = progress, CancellationToken.None);
            await LoadComponentAsync(() => _isGameDataInitComplete, progress => LoadingProgress[LoadingModule.GameData] = progress, CancellationToken.None);
            
            await loadedGameTask;

            _splashSceneView.ShowLoadingCompleted();
        }

        public async UniTask LoadGame()
        {
            await SignIn();
            _isGameDataInitComplete = true;
        }
        
        private async UniTask SignIn()
        {
            await _playerDataService.LoginWithProvider(AuthProvider.Guest);
        }

        private async UniTask LoadComponentAsync(Func<bool> isCompleteFunc, Action<float> progressCallback, CancellationToken cancellationToken)
        {
            while (!isCompleteFunc())
            {
                var totalProgress = LoadingProgress.Values.Sum() / LoadingProgress.Count;
                _splashSceneView.SetProgress(totalProgress);
                progressCallback(totalProgress);

                await UniTask.DelayFrame(1, cancellationToken: cancellationToken);
            }

            progressCallback(1f);
        }
    }
}