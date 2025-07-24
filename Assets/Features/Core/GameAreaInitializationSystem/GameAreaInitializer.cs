using System;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Features.Core.GameAreaInitializationSystem.Models;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables.Editor;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
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
        private readonly IPlacementSystem _placementSystem;
        
        private IGridManager _gridManager;

        public GameAreaInitializer(Func<IGameView> gameViewGetter, GameAreaConfig gameAreaConfig, IPlacementSystem placementSystem)
        {
            _gameViewGetter = gameViewGetter;
            _gameAreaConfig = gameAreaConfig;
            _placementSystem = placementSystem;
        }
        
        public async UniTask InitializeGameArea(GameContext gameContext)
        {
            _gridManager = _gameViewGetter.Invoke().GameAreaView.GridManager;
            CleanGameAreaFromMocks().Forget();
            
            await UniTask.WaitUntil(() => _gridManager.ValidCells.IsNullOrEmpty() == false);

            foreach (var placeable in _gameAreaConfig.Placeables)
            {
                var model = placeable.Value;
                var result = _placementSystem.TryPlaceOnCell(model, placeable.Key);

                if (!result)
                {
                    Logger.LogError($"Failed to place placeable on tile: {placeable.Key}");
                    continue;
                }
                
                
                // switch (placeable.Value.ObjectType)
                // {
                //     case PlaceableType.MergeableObject:
                //         gameContext.Placeables.Add(model as MergeableModel);
                //         break;
                //     case PlaceableType.CollectibleObject:
                //         gameContext.Placeables.Add(model as CollectibleModel);
                //         break;
                //     case PlaceableType.ProductionEntity:
                //         gameContext.Placeables.Add(model as ProductionObjectModel);
                //         break;
                //     case PlaceableType.ProductionBuilding:
                //         gameContext.Placeables.Add(model as ProductionBuildingModel);
                //         break;
                //     default:
                //         throw new ArgumentOutOfRangeException();
                // };
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