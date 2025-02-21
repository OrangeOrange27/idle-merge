using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Package.ControllersTree.Addons.Logging
{
	public class LoggerControllerRunner<TPayload, TResult> : IControllerRunner<TPayload, TResult>
	{
		private readonly IControllerRunner<TPayload, TResult> _decorator;
		private readonly ILogger _logger;


		public LoggerControllerRunner(ILogger logger, IControllerRunner<TPayload, TResult> decorator)
		{
			_decorator = decorator;
			_logger = logger;
		}

		public ControllerStatus ControllerStatus => _decorator.ControllerStatus;

		public string PathInTree => _decorator.PathInTree;

		public async UniTask Initialize(CancellationToken token)
		{
			_logger.ZLogInformation("[{0}] Start Initialize", PathInTree);
			try
			{
				await _decorator.Initialize(token);
			}
			catch (Exception e)
			{
				ProcessLogException(e, nameof(Initialize));
				throw;
			}

			_logger.ZLogInformation("[{0}] Finish Initialize", PathInTree);
		}

		public async UniTask Start(TPayload payload, CancellationToken token)
		{
			_logger.ZLogInformation("[{0}] Start Start", PathInTree);
			try
			{
				await _decorator.Start(payload, token);
			}
			catch (Exception e)
			{
				ProcessLogException(e, nameof(Start));
				throw;
			}

			_logger.ZLogInformation("[{0}] Finish Start", PathInTree);
		}

		public async UniTask<TResult> Execute(CancellationToken token)
		{
			_logger.ZLogInformation("[{0}] Start Execute", PathInTree);
			try
			{
				var result = await _decorator.Execute(token);
				_logger.ZLogInformation("[{0}] Finish Execute", PathInTree);
				return result;
			}
			catch (Exception e)
			{
				ProcessLogException(e, nameof(Execute));
				throw;
			}
		}

		public async UniTask Stop(CancellationToken token)
		{
			_logger.ZLogInformation("[{0}] Start Stop", PathInTree);
			try
			{
				await _decorator.Stop(token);
			}
			catch (Exception e)
			{
				ProcessLogException(e, nameof(Stop));
				throw;
			}

			_logger.ZLogInformation("[{0}] Finish Stop", PathInTree);
		}

		public async UniTask Dispose(CancellationToken token)
		{
			_logger.ZLogInformation("[{0}] Start Dispose", PathInTree);
			try
			{
				await _decorator.Dispose(token);
			}
			catch (Exception e)
			{
				ProcessLogException(e, nameof(Dispose));
				throw;
			}

			_logger.ZLogInformation("[{0}] Finish Dispose", PathInTree);
		}

		public async UniTask AwaitControllerStatus(ControllerStatus status, CancellationToken token)
		{
			await _decorator.AwaitControllerStatus(status, token);
		}

		public IControllerChildren GetControllerChildren()
		{
			return _decorator.GetControllerChildren();
		}

		private void ProcessLogException(Exception e, string currentControllerState)
		{
			if (e.InnerException != null && e.InnerException.GetType() == typeof(ControllerException))
			{
				//don't send error because this error was already reported in child controller
			}
			else if(e.GetBaseException().IsOperationCanceledException())
			{
				_logger.ZLogWarning("OperationCanceledException in controller {0} during state {1}", PathInTree, currentControllerState);
			}
			else
			{
				_logger.ZLogError(e.GetBaseException(), "Exception in controller {0} during state {1}", PathInTree, currentControllerState);
			}
		}
	}
}
