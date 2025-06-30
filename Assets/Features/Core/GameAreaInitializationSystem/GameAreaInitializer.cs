using System;
using Cysharp.Threading.Tasks;
using Features.Core.GameAreaInitializationSystem.Models;
using Features.Core.GridSystem.Managers;
using Features.Gameplay.View;

namespace Features.Core.GameAreaInitializationSystem
{
    public class GameAreaInitializer : IGameAreaInitializer
    {
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

            foreach (var placeable in _gameAreaConfig.Placeables)
            {
                var tile = _gridManager.GetTile(placeable.Key);
                placeable.Value.OccupiedTiles.Add(tile);
                gameContext.Placeables.Add(placeable.Value);
            }
        }
    }
}