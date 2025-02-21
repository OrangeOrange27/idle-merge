using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree;
using Package.ControllersTree.Abstractions;
using Package.StateMachine.Decorators;
using Package.StateMachine.StateMachineErrorHandler;
using Package.StateMachine.Utils;

namespace Package.StateMachine
{
	public class StateMachineController : IControllerWithPayloadAndReturn<StateMachinePayload, IStateMachineInstruction>,
		IStateMachineInternalApi
	{
		protected const int NoMaxHistorySize = -1;
		private static readonly IStateMachineInstruction DefaultResultOfStateMachine = StateMachineInstruction.None;
		private readonly List<IStateMachineHooks> _hooks = new List<IStateMachineHooks>();

		private readonly List<IStateMachineInstruction> _stackedInstructions = new List<IStateMachineInstruction>();

		private IControllerRunner<IStateMachineInternalApi, EmptyPayloadType>[] _decoratorRunners;
		private IStateMachineErrorHandler _errorHandler;
		private CancellationTokenSource _executeCancellationTokenSource;
		private IStateMachineInstruction _resultStateMachineInstruction;

		// ReSharper disable once MemberCanBePrivate.Global
		protected int MaxHistorySize = NoMaxHistorySize;

		//todo add initalize first state in initalize of state machine and start on start
		//todo add finish flow of last state on stop and dispose

		private bool IsHistoryExceedMaxSize => MaxHistorySize != NoMaxHistorySize && StackSize > MaxHistorySize;
		private int StackSize => _stackedInstructions.Count;

		public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public async UniTask OnStart(StateMachinePayload payload, IControllerResources resources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			_decoratorRunners = payload.Decorators == null
				? Array.Empty<IControllerRunner<IStateMachineInternalApi, EmptyPayloadType>>()
				: payload.Decorators.Invoke(controllerChildren);
			await UniTask.WhenAll(_decoratorRunners.Select(runner => runner.Initialize(token)));
			await UniTask.WhenAll(_decoratorRunners.Select(runner => runner.Start(this, token)));

			if (payload.StateMachineInstruction == StateMachineInstruction.None)
				return;
			_errorHandler = payload.StateMachineErrorHandler;

			AddStateAsNext(payload.StateMachineInstruction);
		}

		public async UniTask<IStateMachineInstruction> Execute(IControllerResources resources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			UniTask.WhenAll(_decoratorRunners.Select(runner => runner.Execute(token))).Forget();

			while (_stackedInstructions.Count > 0 && !token.IsCancellationRequested)
				await RunState(controllerChildren, token);
			token.ThrowIfCancellationRequested();

			return _resultStateMachineInstruction ?? DefaultResultOfStateMachine;
		}

		public UniTask OnStop(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public UniTask OnDispose(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public void AddHooks(IStateMachineHooks instanceToAdd)
		{
			_hooks.Add(instanceToAdd);
		}

		public void ChangeActiveState(IStateMachineInstruction stateMachineInstruction)
		{
			AddStateAsNext(stateMachineInstruction);
			KillCurrentState();
		}

		public void AddStateAsNext(IStateMachineInstruction stateMachineInstruction)
		{
			if (stateMachineInstruction.MultipleInstructions.Any())
			{
				_stackedInstructions.PushRange(stateMachineInstruction.MultipleInstructions);
				return;
			}

			_stackedInstructions.Push(stateMachineInstruction);
		}

		public List<IStateMachineInstruction> GetStateMachineInstructionsInStack()
		{
			return _stackedInstructions;
		}

		public IStateMachineInstruction GetCurrentInstruction()
		{
			return GetStateMachineInstructionsInStack().Peek();
		}

		private void KillCurrentState()
		{
			if (_executeCancellationTokenSource == null) throw new ControllerException("No active state");

			_executeCancellationTokenSource?.Cancel();
		}

		private async UniTask RunState(IControllerChildren controllerChildren, CancellationToken cancellationToken)
		{
			var instructionToTake = _stackedInstructions.Peek();
			if (instructionToTake == StateMachineInstruction.None)
			{
				_stackedInstructions.Pop();
				return;
			}

			if (instructionToTake.GoBackRequested)
			{
				_stackedInstructions.Pop(); //Pop GoBackInstruction
				while (_stackedInstructions.Any() && _stackedInstructions.Peek().IgnoreInHistoryRequested)
					_stackedInstructions.Pop();
				return;
			}

			if (instructionToTake.ExitRequested)
			{
				if (instructionToTake.NestedInstruction != null)
					_resultStateMachineInstruction = instructionToTake.NestedInstruction;

				_stackedInstructions.Clear();
				return;
			}

			if (IsHistoryExceedMaxSize) RemoveOldestInstructionFromStack();


			if (instructionToTake.NextStateFactory != null)
			{
				_executeCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

				var currentState = instructionToTake.NextStateFactory.Invoke(controllerChildren);

				try
				{
					await currentState.Initialize(_executeCancellationTokenSource.Token);
					_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

					await UniTask.WhenAll(_hooks.Select(hook => hook.OnBeforeStart(currentState)));
					await instructionToTake.StartDelegate.Invoke(currentState, _executeCancellationTokenSource.Token);
					_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

					if (currentState is IControllerRunnerReturn<IStateMachineInstruction> runnerWithReturnValue)
					{
						await UniTask.WhenAll(_hooks.Select(hook => hook.OnBeforeExecute(currentState)));
						var nextInstruction =
							await runnerWithReturnValue.Execute(_executeCancellationTokenSource.Token);
						_executeCancellationTokenSource.Token.ThrowIfCancellationRequested();

						AddResultStateAsNext(nextInstruction);
					}
					else
					{
						throw new StateMachineException(
							$"State {currentState.GetType().FullName} has not correct controller type. It should have return value with IStateMachineInstruction. Try to implement IStateController");
					}
				}
				catch (Exception e)
				{
					var stateMachineInstruction = _errorHandler.ProcessException(e);
					AddResultStateAsNext(stateMachineInstruction);
				}
				finally
				{
					_executeCancellationTokenSource.Dispose();
					_executeCancellationTokenSource = null;
				}

				if (!instructionToTake.DontAwaitStopAndDisposeRequested)
					await StopAndDispose(currentState);
				else
					StopAndDispose(currentState).Forget();

				return;
			}

			throw new StateMachineException("Returned instruction has no configuration to decide what to do next");

			async UniTask StopAndDispose(IControllerRunnerBase currentState)
			{
				await UniTask.WhenAll(_hooks.Select(hook => hook.OnBeforeStop(currentState)));
				try
				{
					await currentState.TryRunThroughLifecycleToStop(cancellationToken);
				}
				catch (Exception e)
				{
					_errorHandler.ProcessExceptionAtStopOrDispsoe(e);
				}

				try
				{
					await currentState.TryRunThroughLifecycleToDispose(cancellationToken);
				}
				catch (Exception e)
				{
					_errorHandler.ProcessExceptionAtStopOrDispsoe(e);
				}
			}

			void AddResultStateAsNext(IStateMachineInstruction stateMachineInstructionToAdd)
			{
				if (stateMachineInstructionToAdd.GoBackRequested)
					//reverse for require to remove latest instruction in case of duplicated instructions
					for (var i = _stackedInstructions.Count - 1; i >= 0; i--)
						if (_stackedInstructions[i] == instructionToTake)
						{
							_stackedInstructions.RemoveAt(i);
							break;
						}

				AddStateAsNext(stateMachineInstructionToAdd);
			}
		}

		private void RemoveOldestInstructionFromStack()
		{
			_ = _stackedInstructions.PopBottom();
		}
	}
}
