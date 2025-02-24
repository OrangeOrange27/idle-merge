using System.Diagnostics;
using System.Threading;
using Common.EntryPoint.Initialize;
using Common.TimeService;
using Cysharp.Threading.Tasks;
using Features.Gameplay;
using Features.Gameplay.States;
using Package.ControllersTree;
using Package.ControllersTree.Abstractions;
using Package.StateMachine;
using UnityEngine;
using VContainer;

namespace Common.EntryPoint
{
    public class RootController : IStateController
    {
        private readonly IObjectResolver _objectResolver;

        private bool _initialStateIsLobby;
        private IControllerRunner<StateMachinePayload, IStateMachineInstruction> _stateMachineRunner;
        private readonly Stopwatch _stopWatch;

        public RootController(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;
            _stopWatch = Stopwatch.StartNew();
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;

            controllerChildren.Create<UpdateBootTimeController, EmptyPayloadType, EmptyPayloadType>(_objectResolver).RunToDispose(default, token).Forget();

            await controllerChildren.Create<InitializeGameBeforeAuthController, EmptyPayloadType, EmptyPayloadType>(_objectResolver)
                .RunToDispose(default, token);
            await controllerChildren.Create<InitializeGameAfterAuthController, EmptyPayloadType, EmptyPayloadType>(_objectResolver)
                .RunToDispose(default, token);
            
            var initialState = StateMachineInstruction.GoToMany(
                //add here states that runs one by one 
                StateMachineInstructionSugar.GoTo<RootGameplayState>(_objectResolver));

            _stateMachineRunner = controllerChildren.Create<StateMachineController, StateMachinePayload>(_objectResolver);
            await _stateMachineRunner.Initialize(token);
            await _stateMachineRunner.Start(new StateMachinePayload(initialState), token);
        }

        public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
            IControllerChildren controllerChildren, CancellationToken token)
        {
            return await _stateMachineRunner.Execute(token);
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}