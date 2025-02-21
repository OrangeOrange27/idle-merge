using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;
using VContainer;

namespace Package.StateMachine
{
	public class ExternalControlledStateMachine
	{
		private readonly IControllerChildren _controllerChildren;
		private readonly IObjectResolver _objectResolver;
		private CancellationTokenSource _cancellationTokenSourceForController;
		private IControllerRunnerBase _currentControllerRunner;

		public ExternalControlledStateMachine(IControllerChildren controllerChildren, IObjectResolver objectResolver)
		{
			_controllerChildren = controllerChildren;
			_objectResolver = objectResolver;
		}

		public async UniTask<IStateMachineInstruction> RunState<TController, TPayload>(TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
		{
			if (_currentControllerRunner != null)
			{
				await UniTask.WaitUntil(() => _currentControllerRunner.ControllerStatus != ControllerStatus.Initializing &&
				                              _currentControllerRunner.ControllerStatus != ControllerStatus.Starting);
				_cancellationTokenSourceForController.Cancel();
				_currentControllerRunner.TryRunThroughLifecycleToDispose(default).Forget();
			}

			_cancellationTokenSourceForController = new CancellationTokenSource();

			var token = _cancellationTokenSourceForController.Token;
			var runner = StateMachineInstructionSugar.Create<TController, TPayload, IStateMachineInstruction>(_controllerChildren, _objectResolver);
			_currentControllerRunner = runner;
			await _currentControllerRunner.Initialize(token);
			token.ThrowIfCancellationRequested();
			await runner.Start(payload, token);
			token.ThrowIfCancellationRequested();

			return await runner.Execute(token);
		}
	}
}
