using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Package.StateMachine
{
	public class StateMachineInstruction : IStateMachineInstruction
	{
		public static readonly IStateMachineInstruction None = new StateMachineInstruction();
		public static readonly IStateMachineInstruction GoBack = new StateMachineInstruction { GoBackRequested = true };
		public static readonly IStateMachineInstruction Exit = new StateMachineInstruction { ExitRequested = true };
		private static readonly Type DontAwaitStopAndDisposeAttributeType = typeof(DontAwaitStopAndDisposeAttribute);

		public bool GoBackRequested { get; private set; }
		public bool ExitRequested { get; private set; }
		public IStateMachineInstruction NestedInstruction { get; private set; }

		public IEnumerable<IStateMachineInstruction> MultipleInstructions { get; private set; } =
			Enumerable.Empty<IStateMachineInstruction>();

		public Func<IControllerChildren, IControllerRunnerBase> NextStateFactory { get; private set; }
		public Func<IControllerRunnerBase, CancellationToken, UniTask> StartDelegate { get; private set; }

		public Type GetTypeOfState { get; private set; }

		public bool IgnoreInHistoryRequested { get; private set; }
		public bool DontAwaitStopAndDisposeRequested { get; private set; }


		public static IStateMachineInstruction GoTo(Func<IControllerChildren, IControllerRunnerBase> nextStateFactory,
			Func<IControllerRunnerBase, CancellationToken, UniTask> initializeDelegate, Type type)
		{
			return new StateMachineInstruction
				{ NextStateFactory = nextStateFactory, StartDelegate = initializeDelegate, GetTypeOfState = type};
		}

		public static IStateMachineInstruction GoTo<TController, TPayload, TResult>(Func<TController> factory,
			TPayload payload)
			where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
		{
			var stateMachineInstruction = GoTo(
				children => children.Create<TController, TPayload, TResult>(factory),
				(runnerBase, token) =>
				{
					var runner = (IControllerRunnerPayload<TPayload>)runnerBase;
					return runner.Start(payload, token);
				}, typeof(TController));
			
			if (typeof(TController).IsDefined(DontAwaitStopAndDisposeAttributeType))
				stateMachineInstruction = stateMachineInstruction.DontAwaitStopAndDispose();

			return stateMachineInstruction;
		}


		public static IStateMachineInstruction ExitTo(IStateMachineInstruction instructionToParentStateMachine)
		{
			return new StateMachineInstruction
				{ ExitRequested = true, NestedInstruction = instructionToParentStateMachine };
		}

		public static IStateMachineInstruction GoToMany(IEnumerable<IStateMachineInstruction> instructions)
		{
			return new StateMachineInstruction { MultipleInstructions = instructions };
		}

		public static IStateMachineInstruction GoToMany(params IStateMachineInstruction[] instructions)
		{
			return GoToMany((IEnumerable<IStateMachineInstruction>)instructions);
		}

		public IStateMachineInstruction IgnoreInHistory()
		{
			IgnoreInHistoryRequested = true;
			return this;
		}
		
		public IStateMachineInstruction DontAwaitStopAndDispose()
		{
			DontAwaitStopAndDisposeRequested = true;
			return this;
		}
	}
}