using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Package.ControllersTree
{
	public class RootControllerAccessibility
	{
		public static IControllerChildren Children { get; internal set; }
	}
	
	public sealed class RootController<TPayload, TResult> : IControllerWithPayloadAndReturn<TPayload, TResult>
	{
		private readonly IControllerWithPayloadAndReturn<TPayload, TResult> _controllerToRun;
		
		public RootController(IControllerWithPayloadAndReturn<TPayload, TResult> controllerToRun)
		{
			_controllerToRun = controllerToRun;
		}

		public UniTask OnInitialize(IControllerResources controllerResources, CancellationToken token)
		{
			return _controllerToRun.OnInitialize(controllerResources, token);
		}

		public UniTask OnStart(TPayload payload, IControllerResources controllerResources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			RootControllerAccessibility.Children = controllerChildren;
			return _controllerToRun.OnStart(payload, controllerResources, controllerChildren, token);
		}

		public UniTask<TResult> Execute(IControllerResources controllerResources,
			IControllerChildren controllerChildren, CancellationToken token)
		{
			return _controllerToRun.Execute(controllerResources, controllerChildren, token);
		}

		public UniTask OnStop(CancellationToken token)
		{
			return _controllerToRun.OnStop(token);
		}

		public UniTask OnDispose(CancellationToken token)
		{
			return _controllerToRun.OnDispose(token);
		}
	}
}