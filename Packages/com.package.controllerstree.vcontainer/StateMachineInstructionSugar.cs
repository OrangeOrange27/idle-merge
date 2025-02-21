using Package.ControllersTree.Abstractions;
using VContainer;

namespace Package.StateMachine
{
	public static class StateMachineInstructionSugar
	{
		public static IStateMachineInstruction GoTo<TController, TPayload, TResult>(IObjectResolver factory,
			TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			return StateMachineInstruction.GoTo<TController, TPayload, TResult>(() => factory.Resolve<TController>(),
				payload);
		}

		public static IStateMachineInstruction GoTo<TController, TPayload>(IObjectResolver factory,
			TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
		{
			return GoTo<TController, TPayload, IStateMachineInstruction>(factory,
				payload);
		}

		public static IStateMachineInstruction GoTo<TController>(IObjectResolver factory)
			where TController : IControllerWithPayloadAndReturn<EmptyPayloadType, IStateMachineInstruction>
		{
			return GoTo<TController, EmptyPayloadType, IStateMachineInstruction>(factory, default);
		}
		
		
		public static IStateMachineInstruction ExitTo<TController, TPayload, TResult>(IObjectResolver factory,
			TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			return StateMachineInstruction.ExitTo(GoTo<TController, TPayload, TResult>(factory, payload));
		}

		public static IStateMachineInstruction ExitTo<TController, TPayload>(IObjectResolver factory,
			TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
		{
			return ExitTo<TController, TPayload, IStateMachineInstruction>(factory,
				payload);
		}

		public static IStateMachineInstruction ExitTo<TController>(IObjectResolver factory)
			where TController : IControllerWithPayloadAndReturn<EmptyPayloadType, IStateMachineInstruction>
		{
			return ExitTo<TController, EmptyPayloadType, IStateMachineInstruction>(factory, default);
		}
		

		public static IControllerRunner<TPayload, IStateMachineInstruction> Create<TController, TPayload>(
			this IControllerChildren children, IObjectResolver factory)
			where TController : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
		{
			return children.Create<TController, TPayload, IStateMachineInstruction>(factory.Resolve<TController>);
		}
		
		public static IControllerRunner<TPayload, TResult> Create<TController, TPayload, TResult>(
			this IControllerChildren children, IObjectResolver factory)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			return children.Create<TController, TPayload, TResult>(factory.Resolve<TController>);
		}
		
		public static IControllerRunner<EmptyPayloadType, IStateMachineInstruction> Create<TController>(
			this IControllerChildren children, IObjectResolver factory)
			where TController : IControllerWithPayloadAndReturn<EmptyPayloadType, IStateMachineInstruction>
		{
			return children.Create<TController, EmptyPayloadType, IStateMachineInstruction>(factory);
		}
	}
}
