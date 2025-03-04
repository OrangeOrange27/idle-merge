using System;
using Features.Core;
using Features.Core.GridSystem.Managers;
using Features.Core.MergeSystem.Scripts;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
using Features.Gameplay.View;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using R3;

namespace Features.Gameplay.Scripts.Controllers
{
    public class GameplayController : IGameplayController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameplayController>();

        private readonly ISelectionController _selectionController;
        private readonly IPlacementSystem _placementSystem;
        private readonly IMergeController _mergeController;
        private readonly Func<IGameView> _gameViewGetter;

        private GameContext _gameContext;
        private IGridManager _gridManager;

        public GameContext GameContext => _gameContext;
        
        public GameplayController(ISelectionController selectionController, IPlacementSystem placementSystem, Func<IGameView> gameViewGetter, IMergeController mergeController)
        {
            _selectionController = selectionController;
            _placementSystem = placementSystem;
            _gameViewGetter = gameViewGetter;
            _mergeController = mergeController;
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
            if(result.IsSuccessful)
                return;
            
            var targetTile = _gridManager.GetTile(result.TargetCell);
            if(targetTile is not { IsOccupied: true })
                return;

            if (result.Placeable.CanMergeWith(targetTile.OccupyingObject))
            {
                _mergeController.TryMerge(_gameContext, result.Placeable, targetTile);
            }
        }

        public void RegisterPlaceableClick(PlaceableModel placeableModel)
        {
            Logger.LogInformation($"Placeable {placeableModel} clicked");
            _selectionController.SelectPlaceable(placeableModel);
        }
    }
}