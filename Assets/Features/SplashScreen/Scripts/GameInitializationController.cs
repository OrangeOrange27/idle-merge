using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.EntryPoint.Initialize;
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
        private readonly GameLoadingManager _gameLoadingManager;

        private static readonly Dictionary<LoadingModule, float> LoadingProgress = new()
        {
            { LoadingModule.PlayerData, 0.5f },
            { LoadingModule.GameData, 0.5f }
        };

        public GameInitializationController(SplashSceneView splashSceneView, GameLoadingManager gameLoadingManager)
        {
            _splashSceneView = splashSceneView;
            _gameLoadingManager = gameLoadingManager;
        }

        public async UniTask InitializeBeforeAuth()
        {
            await LoadComponentAsync(() => _gameLoadingManager.IsSignInDone, progress => LoadingProgress[LoadingModule.PlayerData] = progress, CancellationToken.None);
            await LoadComponentAsync(() => _gameLoadingManager.IsGameDataInitComplete, progress => LoadingProgress[LoadingModule.GameData] = progress, CancellationToken.None);

            _splashSceneView.ShowLoadingCompleted();
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