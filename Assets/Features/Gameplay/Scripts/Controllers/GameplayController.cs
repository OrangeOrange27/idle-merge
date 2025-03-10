using System;
using System.Collections.Generic;
using System.Linq;
using Common.PlayerData;
using Features.Core;
using Features.Core.GridSystem.Managers;
using Features.Core.MergeSystem;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
using Features.Gameplay.View;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using R3;
using ZLogger;

namespace Features.Gameplay.Scripts.Controllers
{
    public class GameplayController : IGameplayController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameplayController>();

        private readonly ISelectionController _selectionController;
        private readonly IPlacementSystem _placementSystem;
        private readonly IMergeController _mergeController;
        private readonly IPlayerDataService _playerDataService;
        private readonly Func<IGameView> _gameViewGetter;

        private GameContext _gameContext;
        private IGridManager _gridManager;

        public GameContext GameContext => _gameContext;

        public GameplayController(ISelectionController selectionController, IPlacementSystem placementSystem,
            Func<IGameView> gameViewGetter, IMergeController mergeController, IPlayerDataService playerDataService)
        {
            _selectionController = selectionController;
            _placementSystem = placementSystem;
            _gameViewGetter = gameViewGetter;
            _mergeController = mergeController;
            _playerDataService = playerDataService;
        }

        public IDisposable Initialize(GameContext gameContext)
        {
            _gameContext = gameContext;
            var disposableBag = new DisposableBag();

            _gridManager = _gameViewGetter.Invoke().GameAreaView.GridManager;
            disposableBag.Add(_placementSystem.Initialize(_gameContext, _gridManager));

            _placementSystem.OnPlacementAttempt += OnPlacementAttempt;
            disposableBag.Add(Disposable.Create(() => _placementSystem.OnPlacementAttempt -= OnPlacementAttempt));

            return disposableBag;
        }

        private void OnPlacementAttempt(PlacementRequestResult result)
        {
            if (result.IsSuccessful)
                return;

            var targetTile = _gridManager.GetTile(result.TargetCell);
            if (targetTile is not { IsOccupied: true })
                return;

            if (result.Placeable is MergeableModel resultMergeable)
            {
                if (resultMergeable.CanMergeWith(targetTile.OccupyingObject))
                {
                    _mergeController.TryMerge(_gameContext, resultMergeable, targetTile);
                }
            }
        }

        public void RegisterPlaceableClick(PlaceableModel placeableModel)
        {
            Logger.LogInformation($"Placeable {placeableModel} clicked");
            switch (placeableModel.ObjectType)
            {
                case PlaceableType.ProductionBuilding:
                case PlaceableType.SpecialObject:
                    break;
                case PlaceableType.MergeableObject:
                    RegisterClickOnMergeable(placeableModel as MergeableModel);
                    break;
                case PlaceableType.CollectibleObject:
                    RegisterClickOnCollectible(placeableModel as CollectibleModel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void RegisterClickOnMergeable(MergeableModel placeableModel)
        {
            if (placeableModel == null)
            {
                Logger.ZLogWarning("Tried to register click on NULL mergeable");
                return;
            }
            
            _selectionController.SelectPlaceable(placeableModel);
        }

        private void RegisterClickOnCollectible(CollectibleModel placeableModel)
        {
            if (placeableModel == null)
            {
                Logger.ZLogWarning("Tried to register click on NULL collectible");
                return;
            }
            
            var collectibleType = placeableModel.CollectibleType;
            var collectibles = GetAllCollectiblesOnGameArea(collectibleType); 
            if(collectibles == null || collectibles.Count <=0)
                return;
            
            _playerDataService.GiveCollectible(collectibleType, collectibles.Count);
        }

        private List<CollectibleModel> GetAllCollectiblesOnGameArea(CollectibleType type)
        {
            return _gameContext.Placeables
                .OfType<CollectibleModel>()
                .Where(model => model.CollectibleType == type)
                .ToList();
        }
    }
}