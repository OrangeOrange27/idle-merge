using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;

namespace Package.ControllersTree.Addons.Abstractions
{
	public class OnDemandControllerRunner<TPayload, TResult> : IControllerRunner<TPayload, TResult>
	{
		private readonly IControllerWithPayloadAndReturn<TPayload, TResult> _controller;
		private readonly IControllerRunner<TPayload, TResult> _decorator;
		private bool _skipController;
		private TResult _defaultValueForResult;

		public OnDemandControllerRunner(IControllerWithPayloadAndReturn<TPayload, TResult> controller,
			IControllerRunner<TPayload, TResult> decorator)
		{
			_controller = controller;
			_decorator = decorator;
		}
		
		public string PathInTree => _decorator.PathInTree;
		public ControllerStatus ControllerStatus => _decorator.ControllerStatus;

		public UniTask Initialize(CancellationToken token)
		{
			return _decorator.Initialize(token);
		}

		public UniTask Start(TPayload payload, CancellationToken token)
		{
			if (_controller is IOnDemandController<TResult> onDemandController &&
			    !onDemandController.ShouldControllerRun())
			{
				_skipController = true;
				_defaultValueForResult = onDemandController.GetDefaultResult();
			}

			if (_skipController)
				return UniTask.CompletedTask;

			return _decorator.Start(payload, token);
		}

		public UniTask<TResult> Execute(CancellationToken token)
		{
			if (_skipController)
				return UniTask.FromResult(_defaultValueForResult);
			return _decorator.Execute(token);
		}

		public UniTask Stop(CancellationToken token)
		{
			if (_skipController)
				return UniTask.CompletedTask;
			return _decorator.Stop(token);
		}

		public UniTask Dispose(CancellationToken token)
		{
			return _decorator.Dispose(token);
		}
        
		public UniTask AwaitControllerStatus(ControllerStatus status, CancellationToken token)
		{
			return _decorator.AwaitControllerStatus(status, token);
		}
		
		public IControllerChildren GetControllerChildren()
		{
			return _decorator.GetControllerChildren();
		}
	}
}
