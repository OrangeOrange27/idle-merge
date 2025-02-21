using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Package.ControllersTree.Implementations
{
	public class ControllerRunner<TPayload, TResult> : IControllerRunner<TPayload, TResult>
	{
		private readonly IControllerWithPayloadAndReturn<TPayload, TResult> _controller;
		private readonly IControllerChildren _controllerChildren;
		private readonly IControllerResources _controllerResources;

		private readonly UniTaskCompletionSource _stoppedStatusCallback = new UniTaskCompletionSource();
		private readonly UniTaskCompletionSource _disposedStatusCallback = new UniTaskCompletionSource();
		private List<Exception> _exceptions;

		public ControllerRunner(string parentPathInTree, IControllerWithPayloadAndReturn<TPayload, TResult> controller,
			Func<IControllerRunnerBase, IControllerChildren>  controllerChildren, IControllerResources controllerResources)
		{
			_controller = controller;
			_controllerChildren = controllerChildren.Invoke(this);
			_controllerResources = controllerResources;
			PathInTree = parentPathInTree + "/" + controller.GetType().Name;
		}

		public ControllerStatus ControllerStatus { get; private set; } = ControllerStatus.Created;
		public string PathInTree { get; }


		public async UniTask Initialize(CancellationToken token)
		{
			CheckCurrentControllerState(ControllerStatus.Created);
			ControllerStatus = ControllerStatus.Initializing;
			try
			{
				await SafetyInvokeAsync(
					() => _controller.OnInitialize(_controllerResources, token));
				ThrowAggregateExceptionIfHappen();
			}
			catch
			{
				ControllerStatus = ControllerStatus.FailedAtInitialize;
				throw;
			}

			ControllerStatus = ControllerStatus.Initialized;
		}

		public async UniTask Start(TPayload payload, CancellationToken token)
		{
			CheckCurrentControllerState(ControllerStatus.Initialized);
			ControllerStatus = ControllerStatus.Starting;
			try
			{
				await SafetyInvokeAsync(
					() => _controller.OnStart(payload, _controllerResources, _controllerChildren, token));
				ThrowAggregateExceptionIfHappen();
			}
			catch
			{
				ControllerStatus = ControllerStatus.FailedAtStart;
				throw;
			}

			ControllerStatus = ControllerStatus.Started;
		}


		public async UniTask<TResult> Execute(CancellationToken token)
		{
			CheckCurrentControllerState(ControllerStatus.Started);
			ControllerStatus = ControllerStatus.Executing;
			TResult result = default;

			try
			{
				await SafetyInvokeAsync(
					async () => result = await _controller.Execute(_controllerResources, _controllerChildren, token));
				ThrowAggregateExceptionIfHappen();
			}
			catch
			{
				ControllerStatus = ControllerStatus.FailedAtExecution;
				throw;
			}

			ControllerStatus = ControllerStatus.Executed;

			return result;
		}

		public async UniTask Stop(CancellationToken token)
		{
			CheckCurrentControllerState(ControllerStatus.Started, ControllerStatus.FailedAtExecution,
				ControllerStatus.Executed);
			ControllerStatus = ControllerStatus.Stopping;

			try
			{
				await RunChildrenControllersToTargetStatus(ControllerStatus, token);
				await SafetyInvokeAsync(() => _controller.OnStop(token));
				ThrowAggregateExceptionIfHappen();
			}
			catch
			{
				ControllerStatus = ControllerStatus.FailedAtStop;
				throw;
			}

			ControllerStatus = ControllerStatus.Stopped;
			_stoppedStatusCallback.TrySetResult();
		}

		public async UniTask Dispose(CancellationToken token)
		{
			CheckCurrentControllerState(ControllerStatus.Initialized,
				ControllerStatus.FailedAtStart, ControllerStatus.Started, ControllerStatus.FailedAtExecution,
				ControllerStatus.Executed, ControllerStatus.FailedAtStop, ControllerStatus.Stopped);
			ControllerStatus = ControllerStatus.Disposing;

			try
			{
				await RunChildrenControllersToTargetStatus(ControllerStatus, token);
				await SafetyInvokeAsync(() => _controller.OnDispose(token));
				SafetyInvoke(() => _controllerResources.Dispose());
				ThrowAggregateExceptionIfHappen();
			}
			finally
			{
				ControllerStatus = ControllerStatus.Disposed;
				_disposedStatusCallback.TrySetResult();
			}
		}


		private async UniTask RunChildrenControllersToTargetStatus(ControllerStatus controllerStatus,
			CancellationToken token)
		{
			if (controllerStatus >= ControllerStatus.Stopping)
			{
				await SafetyInvokeAsync(() => UniTask.WhenAll(Enumerable.Select(_controllerChildren
					.GetChildrenRunners(_controller)
					.Where(runner => runner.ControllerStatus >= ControllerStatus.Started &&
					                 runner.ControllerStatus <
					                 ControllerStatus
						                 .Stopping // we exclude already stopping because we cannot invoke Lifecycle method twice
					), runner => runner.Stop(token))));


				//await forgot stop running
				await SafetyInvokeAsync(() => UniTask.WhenAll(_controllerChildren
					.GetChildrenRunners(_controller)
					.Where(runner => runner.ControllerStatus == ControllerStatus.Stopping)
					.Cast<IControllerRunnerAwaitableCallbacks>()
					.Select(runner => runner.AwaitControllerStatus(ControllerStatus.Stopped, token))));
			}

			if (controllerStatus >= ControllerStatus.Disposing)
			{
				await SafetyInvokeAsync(() => UniTask.WhenAll(Enumerable.Select(_controllerChildren
					.GetChildrenRunners(_controller)
					.Where(runner => runner.ControllerStatus >= ControllerStatus.Initialized &&
					                 runner.ControllerStatus <
					                 ControllerStatus
						                 .Disposing // we exclude already Disposing because we cannot invoke Lifecycle method twice
					), runner => runner.Dispose(token))));

				//await forgot dispose running
				await SafetyInvokeAsync(() => UniTask.WhenAll(_controllerChildren
					.GetChildrenRunners(_controller)
					.Where(runner => runner.ControllerStatus == ControllerStatus.Disposing)
					.Cast<IControllerRunnerAwaitableCallbacks>()
					.Select(runner => runner.AwaitControllerStatus(ControllerStatus.Disposed, token))));
			}
		}

		private void CheckCurrentControllerState(params ControllerStatus[] possibleControllerStatus)
		{
			if (possibleControllerStatus.All(possible => possible != ControllerStatus))
				throw new ControllerException(
					$"Lifecycle violation. You should be at one of the next statuses {string.Join(", ", possibleControllerStatus)}. Current is {ControllerStatus}");
		}

		private void ThrowAggregateExceptionIfHappen()
		{
			if (_exceptions != null && _exceptions.Count > 0)
			{
				var innerException = _exceptions.Count > 1 ? new AggregateException(_exceptions.ToArray()) : _exceptions[0];
				var aggregateException = new ControllerException(
					$"Exception in controller {_controller.GetType().FullName} during state {ControllerStatus}",
					innerException);

				_exceptions.Clear();
				throw aggregateException;
			}
		}

		private void SafetyInvoke(Action action)
		{
			try
			{
				action.Invoke();
			}
			catch (Exception e)
			{
				_exceptions ??= new List<Exception>();
				_exceptions.Add(e);
			}
		}

		private async UniTask SafetyInvokeAsync(Func<UniTask> action)
		{
			try
			{
				await action.Invoke();
			}
			catch (Exception e)
			{
				_exceptions ??= new List<Exception>();
				_exceptions.Add(e);
			}
		}

		public UniTask AwaitControllerStatus(ControllerStatus status, CancellationToken token)
		{
			switch (status)
			{
				case ControllerStatus.Stopped:
					return _stoppedStatusCallback.Task;
				case ControllerStatus.Disposed:
					return _disposedStatusCallback.Task;
				default:
					throw new NotSupportedException(
						$"{status} is not supported. Need to add this support. I didn't implement it for now to not make more overhead for every controller run");
			}
		}

		public IControllerChildren GetControllerChildren()
		{
			return _controllerChildren;
		}
	}
}
