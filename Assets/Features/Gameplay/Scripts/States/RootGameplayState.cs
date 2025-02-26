using System.Threading;
using Cysharp.Threading.Tasks;
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
        
        private IGameUIView _gameUIView;
        private IGameAreaView _gameAreaView;

        public RootGameplayState(ISharedViewLoader<IGameView> gameViewLoader)
        {
            _gameViewLoader = gameViewLoader;
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
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _machineInstructionCompletionSource.Task.AttachExternalCancellation(token);
        }

        public async UniTask OnStop(CancellationToken token)
        {
        }

        public async UniTask OnDispose(CancellationToken token)
        {
        }
    }
}