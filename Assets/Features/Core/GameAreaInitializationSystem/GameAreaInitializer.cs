using System;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Features.Core.GameAreaInitializationSystem.Models;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables.Editor;
using Features.Gameplay.View;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;
using Object = UnityEngine.Object;

namespace Features.Core.GameAreaInitializationSystem
{
    public class GameAreaInitializer : IGameAreaInitializer
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameAreaInitializer>();
        
        private readonly Func<IGameView> _gameViewGetter;
        private readonly GameAreaConfig _gameAreaConfig;
        
        private IGridManager _gridManager;

        public GameAreaInitializer(Func<IGameView> gameViewGetter, GameAreaConfig gameAreaConfig)
        {
            _gameViewGetter = gameViewGetter;
            _gameAreaConfig = gameAreaConfig;
        }
        
        public async UniTask InitializeGameArea(GameContext gameContext)
        {
            _gridManager = _gameViewGetter.Invoke().GameAreaView.GridManager;
            CleanGameAreaFromMocks().Forget();
            
            await UniTask.WaitUntil(() => _gridManager.ValidCells.IsNullOrEmpty() == false);

            foreach (var placeable in _gameAreaConfig.Placeables)
            {
                var tile = _gridManager.GetTile(placeable.Key);
                if (tile == null)
                {
                    Logger.ZLogError($"Tile {placeable.Key} does not exist");
                    continue;
                }
                placeable.Value.OccupiedTiles.Add(tile);
                gameContext.Placeables.Add(placeable.Value);
            }
        }
        
        //temp method for cleaning game area from mock objects 
        private async UniTask CleanGameAreaFromMocks()
        {
            var mocks = _gridManager.Grid
                .GetComponentsInChildren<ProductionBuildingEditorModel>();

            for (int i = 0; i < mocks.Length; i++)
            {
                Object.Destroy(mocks[i].gameObject);
            }
        }
    }
}