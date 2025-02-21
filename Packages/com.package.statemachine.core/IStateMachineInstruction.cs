using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Package.StateMachine
{
	public interface IStateMachineInstruction : IStateMachineInstructionMultiple, IStateMachineInstructionGoToNextState,
		IStateMachineInstructionExit, IStateMachineInstructionGoBack, IStateMachineInstructionIgnoreInHistory,
		IStateMachineInstructionDontAwaitStop
	{
	}

	public interface IStateMachineInstructionMultiple
	{
		IEnumerable<IStateMachineInstruction> MultipleInstructions { get; }
	}

	public interface IStateMachineInstructionGoToNextState
	{
		Func<IControllerChildren, IControllerRunnerBase> NextStateFactory { get; }
		Func<IControllerRunnerBase, CancellationToken, UniTask> StartDelegate { get; }
		Type GetTypeOfState { get; }
	}

	public interface IStateMachineInstructionExit
	{
		bool ExitRequested { get; }
		IStateMachineInstruction NestedInstruction { get; }
	}

	public interface IStateMachineInstructionGoBack
	{
		bool GoBackRequested { get; }
	}

	public interface IStateMachineInstructionIgnoreInHistory
	{
		bool IgnoreInHistoryRequested { get; }
		IStateMachineInstruction IgnoreInHistory();
	}

	public interface IStateMachineInstructionDontAwaitStop
	{
		bool DontAwaitStopAndDisposeRequested { get; }
		IStateMachineInstruction DontAwaitStopAndDispose();
	}
}
