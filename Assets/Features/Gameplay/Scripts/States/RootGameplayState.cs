using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core;
using Features.Core.GridSystem.Managers;
using Features.Core.PlacementSystem;
using Features.Core.SupplySystem;
using Features.Gameplay.Scripts.Controllers;
using Features.Gameplay.View;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using Package.StateMachine;

namespace Features.Gameplay.States
{
    public class RootGameplayState : IStateController
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();
        
        private readonly ISharedViewLoader<IGameView> _gameViewLoader;
        private readonly ISupplyManager _supplyManager;
        private readonly IPlaceablesVisualSystem _placeablesVisualSystem;
        private readonly IGameplayController _gameplayController;
        private readonly IPlacementSystem _placementSystem;

        private IGameUIView _gameUIView;
        private IGameAreaView _gameAreaView;

        public RootGameplayState(ISharedViewLoader<IGameView> gameViewLoader, ISupplyManager supplyManager,
            IPlaceablesVisualSystem placeablesVisualSystem, IGameplayController gameplayController,
            IPlacementSystem placementSystem)
        {
            _gameViewLoader = gameViewLoader;
            _supplyManager = supplyManager;
            _placeablesVisualSystem = placeablesVisualSystem;
            _gameplayController = gameplayController;
            _placementSystem = placementSystem;
        }

        public async UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
        }
        
        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            var gameView = await _gameViewLoader.Load(resources, token, null);
            _gameUIView = gameView.GameUIView;
            _gameAreaView = gameView.GameAreaView;
            
            _gameUIView.OnSupplyButtonClicked += OnSupplyClick;
            
            resources.Attach(_gameplayController.Initialize(new GameContext())); //todo: get valid context
            resources.Attach(_placementSystem.Initialize(_gameplayController.GameContext));

            await _placeablesVisualSystem.SpawnInitPlaceablesViews(_gameplayController.GameContext, resources, token);
            await _placeablesVisualSystem.InitializePlaceablesViews();
        }

        private void OnSupplyClick()
        {
            _supplyManager.SpawnSupply(_gameplayController.GameContext);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public async UniTask OnStop(CancellationToken token)
        {
            _gameUIView.OnSupplyButtonClicked -= OnSupplyClick;
        }

        public async UniTask OnDispose(CancellationToken token)
        {
        }
    }
}