using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;
using VContainer;

namespace Package.ControllersTree.Addons.ExtensionPoints
{
	public class ExtensionPointsControllerRunner<TController, TPayload, TResult> : IControllerRunner<TPayload, TResult>
		where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
	{
		private readonly IControllerRunner<TPayload, TResult> _decorator;
		private readonly IObjectResolver _objectResolver;
		private List<IControllerRunner<TPayload, TResult>> _subControllerRunners;

		public ExtensionPointsControllerRunner(IObjectResolver objectResolver,
			IControllerRunner<TPayload, TResult> decorator)
		{
			_objectResolver = objectResolver;
			_decorator = decorator;
		}

		public string PathInTree => _decorator.PathInTree;
		public ControllerStatus ControllerStatus => _decorator.ControllerStatus;

		public async UniTask Initialize(CancellationToken token)
		{
			var subControllers = _objectResolver.Resolve<IReadOnlyList<ISubController<TController, TPayload, TResult>>>();
			_subControllerRunners = subControllers.Select(subController =>
				_decorator.GetControllerChildren().Create<ISubController<TController, TPayload, TResult>, TPayload, TResult>(() => subController)).ToList();

			await UniTask.WhenAll(_subControllerRunners.Select(SafeSubControllerRun));

			await _decorator.Initialize(token);

			async UniTask SafeSubControllerRun(IControllerRunner<TPayload, TResult> runner)
			{
				try
				{
					await runner.Initialize(token);
				}
				catch (Exception _)
				{
					//don't throw exception because subcontroller shouldn't influence on parent controller
				}
			}
		}

		public async UniTask Start(TPayload payload, CancellationToken token)
		{
			await UniTask.WhenAll(_subControllerRunners.Select(SafeSubControllerRun));

			await _decorator.Start(payload, token);

			async UniTask SafeSubControllerRun(IControllerRunner<TPayload, TResult> runner)
			{
				if (runner.ControllerStatus != ControllerStatus.Initialized)
					return;
				try
				{
					await runner.Start(payload, token);
				}
				catch (Exception _)
				{
					//don't throw exception because subcontroller shouldn't influence on parent controller
				}
			}
		}

		public async UniTask<TResult> Execute(CancellationToken token)
		{
			var cts = CancellationTokenSource.CreateLinkedTokenSource(token);

			var completionSource = new UniTaskCompletionSource<TResult>();
			var subControllerUniTasks = _subControllerRunners
				.Select(runner => runner.Execute(cts.Token)
					.ContinueWith(controllerResult =>
					{
						if (controllerResult != null && !controllerResult.Equals(default(TResult))) completionSource.TrySetResult(controllerResult);
					}).Preserve()).
				ToArray();
			
			var parentControllerTask = ExecuteDecoratorAndProxyCompletionSource();

			TResult result;
			try
			{
				result = await completionSource.Task;
			}
			finally
			{
				cts.Cancel();

				await WaitUntilEndAllControllers();
			}

			return result;

			async Task WaitUntilEndAllControllers()
			{
				try
				{
					await UniTask.WhenAll(subControllerUniTasks);
					await parentControllerTask;
				}
				catch (ControllerException _)
				{
					// ignored to not interupt flow after getting results of execution
				}
			}
			
			async UniTask ExecuteDecoratorAndProxyCompletionSource()
			{
				TResult resultExecute;
				try
				{
					resultExecute = await _decorator.Execute(cts.Token);
				}
				catch (Exception e)
				{
					completionSource.TrySetException(e);
					return;
				}
				
				completionSource.TrySetResult(resultExecute);
			}
		}


		public async UniTask Stop(CancellationToken token)
		{
			await UniTask.WhenAll(_subControllerRunners.Select(SafeSubControllerRun));

			await _decorator.Stop(token);

			async UniTask SafeSubControllerRun(IControllerRunner<TPayload, TResult> runner)
			{
				try
				{
					await runner.Stop(token);
				}
				catch (Exception _)
				{
					//don't throw exception because subcontroller shouldn't influence on parent controller
				}
			}
		}

		public async UniTask Dispose(CancellationToken token)
		{
			await UniTask.WhenAll(_subControllerRunners.Select(SafeSubControllerRun));
			await _decorator.Dispose(token);

			async UniTask SafeSubControllerRun(IControllerRunner<TPayload, TResult> runner)
			{
				try
				{
					await runner.Dispose(token);
				}
				catch (Exception _)
				{
					//don't throw exception because subcontroller shouldn't influence on parent controller
				}
			}
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
