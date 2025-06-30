using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core;
using Features.Core.GameAreaInitializationSystem;
using Features.Core.GridSystem.Managers;
using Features.Core.Placeables.VisualSystem;
using Features.Core.PlacementSystem;
using Features.Core.SupplySystem;
using Features.Gameplay.Scripts.Controllers;
using Features.Gameplay.View;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using Package.StateMachine;
using VContainer;

namespace Features.Gameplay.States
{
    public class RootGameplayState : IStateController
    {
        private readonly UniTaskCompletionSource<IStateMachineInstruction> _machineInstructionCompletionSource = new();
        
        private readonly ISharedViewLoader<IGameView> _gameViewLoader;
        private readonly ISupplyManager _supplyManager;
        private readonly IPlaceablesVisualSystem _placeablesVisualSystem;
        private readonly IGameplayController _gameplayController;
        private readonly IObjectResolver _resolver;
        private readonly IGameAreaInitializer _gameAreaInitializer;

        private IGameUIView _gameUIView;
        private IGameAreaView _gameAreaView;

        public RootGameplayState(IObjectResolver resolver, ISharedViewLoader<IGameView> gameViewLoader, ISupplyManager supplyManager,
            IPlaceablesVisualSystem placeablesVisualSystem, IGameplayController gameplayController, IGameAreaInitializer gameAreaInitializer)
        {
            _resolver = resolver;
            _gameViewLoader = gameViewLoader;
            _supplyManager = supplyManager;
            _placeablesVisualSystem = placeablesVisualSystem;
            _gameplayController = gameplayController;
            _gameAreaInitializer = gameAreaInitializer;
        }

        public async UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
        }
        
        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token)
        {
            var gameView = await _gameViewLoader.Load(resources, token, null);
            gameView.Initialize(_resolver);
            _gameUIView = gameView.GameUIView;
            _gameAreaView = gameView.GameAreaView;

            var gameContext = new GameContext(); //todo: get valid context
            
            await _gameAreaInitializer.InitializeGameArea(gameContext);
            
            resources.Attach(_gameplayController.Initialize(gameContext)); 
            await _placeablesVisualSystem.SpawnInitPlaceablesViews(_gameplayController.GameContext, resources, token);
            await _placeablesVisualSystem.InitializePlaceablesViews();
            
            _gameUIView.OnSupplyButtonClicked += OnSupplyClick;
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