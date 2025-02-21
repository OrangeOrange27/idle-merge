using System;
using Package.ControllersTree.Abstractions;
using Package.StateMachine.StateMachineErrorHandler;

namespace Package.StateMachine
{
	public struct StateMachinePayload
	{
		public IStateMachineInstruction StateMachineInstruction { get; }
		public Func<IControllerChildren, IControllerRunner<IStateMachineInternalApi, EmptyPayloadType>[]> Decorators { get; }
		public IStateMachineErrorHandler StateMachineErrorHandler { get; }

		public StateMachinePayload(IStateMachineInstruction stateMachineInstruction)
		{
			StateMachineInstruction = stateMachineInstruction;
			StateMachineErrorHandler = new DefaultStateMachineErrorHandler();
			Decorators = null;
		}

		public StateMachinePayload(IStateMachineInstruction stateMachineInstruction,
			Func<IControllerChildren, IControllerRunner<IStateMachineInternalApi, EmptyPayloadType>[]> decorators = null,
			IStateMachineErrorHandler errorHandler = null)
		{
			StateMachineInstruction = stateMachineInstruction;
			StateMachineErrorHandler = errorHandler ?? new DefaultStateMachineErrorHandler();
			Decorators = decorators;
		}
	}
}
