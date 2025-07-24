using System;
using System.Collections.Generic;
using Common.Utils.Extensions;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Models;
using Features.Core.GameAreaInitializationSystem.Models;
using Features.Core.GridSystem.Managers;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Editor;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
using Features.Core.ProductionSystem.Models;
using Features.Gameplay.View;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Object = UnityEngine.Object;

namespace Features.Core.GameAreaInitializationSystem
{
    public class TEMP_GameAreaInitializer : IGameAreaInitializer
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameAreaInitializer>();
        
        private readonly Func<IGameView> _gameViewGetter;
        private readonly GameAreaConfig _gameAreaConfig;
        private readonly IPlacementSystem _placementSystem;
        
        private IGridManager _gridManager;

        public TEMP_GameAreaInitializer(Func<IGameView> gameViewGetter, GameAreaConfig gameAreaConfig, IPlacementSystem placementSystem)
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

                model.AvailableRecipes = CreateRecipes();
                gameContext.Placeables.Add(model);
            }
        }

        private ProductionRecipe[] CreateRecipes()
        {
            return new ProductionRecipe[]
            {
                new()
                {
                    CraftingTimeInSeconds = 5,
                    Ingredients = new List<CollectibleItemUIModel>()
                    {
                        new()
                        {
                            Amout = 3,
                            CollectibleType = CollectibleType.Fur
                        }
                    },
                    Outcome = new List<MergeableItemUIModel>()
                    {
                        new()
                        {
                            MergeableType = MergeableType.Coin,
                            Tier = 1
                        }
                    },
                    RecipeName = "recipe_1"
                },
                new()
                {
                    CraftingTimeInSeconds = 5,
                    Ingredients = new List<CollectibleItemUIModel>()
                    {
                        new()
                        {
                            Amout = 3,
                            CollectibleType = CollectibleType.Milk
                        }
                    },
                    Outcome = new List<MergeableItemUIModel>()
                    {
                        new()
                        {
                            MergeableType = MergeableType.Coin,
                            Tier = 1
                        }
                    },
                    RecipeName = "recipe_2"
                },
                new()
                {
                    CraftingTimeInSeconds = 5,
                    Ingredients = new List<CollectibleItemUIModel>()
                    {
                        new()
                        {
                            Amout = 3,
                            CollectibleType = CollectibleType.Fish
                        }
                    },
                    Outcome = new List<MergeableItemUIModel>()
                    {
                        new()
                        {
                            MergeableType = MergeableType.Coin,
                            Tier = 1
                        }
                    },
                    RecipeName = "recipe_3"
                },
            };
        }
        
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