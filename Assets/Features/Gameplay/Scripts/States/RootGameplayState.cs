using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.MergeSystem.Controller;
using Features.Core.SupplySystem;
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
        private readonly IMergeController _mergeController;
        
        private IGameUIView _gameUIView;
        private IGameAreaView _gameAreaView;

        public RootGameplayState(ISharedViewLoader<IGameView> gameViewLoader, ISupplyManager supplyManager, IMergeController mergeController)
        {
            _gameViewLoader = gameViewLoader;
            _supplyManager = supplyManager;
            _mergeController = mergeController;
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
            
            _gameUIView.OnSupplyButtonClicked += OnSupplyButtonClicked;
        }

        private void OnSupplyButtonClicked()
        {
            _supplyManager.SpawnSupply();
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public async UniTask OnStop(CancellationToken token)
        {
            _gameUIView.OnSupplyButtonClicked -= OnSupplyButtonClicked;
        }

        public async UniTask OnDispose(CancellationToken token)
        {
        }
    }
}