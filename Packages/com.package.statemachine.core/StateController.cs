using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Package.StateMachine
{
	public abstract class StateController : StateController<EmptyPayloadType>
	{
	}
	
	public abstract class StateController<T> : IStateController<T>
	{
		[SerializeField] protected readonly UniTaskCompletionSource<IStateMachineInstruction> StateComplitionSource = new();

		public virtual UniTask OnInitialize(IControllerResources resources, CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public virtual UniTask OnStart(T payload, IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public virtual async UniTask<IStateMachineInstruction> Execute(IControllerResources resources, IControllerChildren controllerChildren,
			CancellationToken token)
		{
			return await StateComplitionSource.Task.AttachExternalCancellation(token);
		}

		public virtual UniTask OnStop(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}

		public virtual UniTask OnDispose(CancellationToken token)
		{
			return UniTask.CompletedTask;
		}
	}
}
