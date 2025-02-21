using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;

namespace Package.ControllersTree
{
	public static class ControllerRunnerExtension
	{
		public static async UniTask Run(
			this IControllerRunner<EmptyPayloadType, EmptyPayloadType> controllerRunner, CancellationToken token)
		{
			await Run(controllerRunner, default, token);
		}

		public static async UniTask<TResult> Run<TPayload, TResult>(
			this IControllerRunner<TPayload, TResult> controllerRunner, TPayload payload, CancellationToken token)
		{
			await controllerRunner.Initialize(token);
			await controllerRunner.Start(payload, token);
			return await controllerRunner.Execute(token);
		}

		public static async UniTask<TResult> RunToDispose<TPayload, TResult>(
			this IControllerRunner<TPayload, TResult> controllerRunner, TPayload payload, CancellationToken token)
		{
			await controllerRunner.Initialize(token);
			await controllerRunner.Start(payload, token);
			var result = await controllerRunner.Execute(token);
			await controllerRunner.Stop(token);
			await controllerRunner.Dispose(token);
			return result;
		}

		public static async UniTask TryRunThroughLifecycleToStop(
			this IControllerRunnerBase controllerRunner, CancellationToken token)
		{
			if (controllerRunner.ControllerStatus >= ControllerStatus.Started && controllerRunner.ControllerStatus < ControllerStatus.Stopping)
				await controllerRunner.Stop(token);
		}

		public static async UniTask TryRunThroughLifecycleToDispose(
			this IControllerRunnerBase controllerRunner, CancellationToken token)
		{
			await controllerRunner.TryRunThroughLifecycleToStop(token);
			if (controllerRunner.ControllerStatus >= ControllerStatus.Initialized && controllerRunner.ControllerStatus < ControllerStatus.Disposing)
				await controllerRunner.Dispose(token);
		}
		
		public static bool IsControllerRunning<T>(this IControllerChildren controllerChildren)
		{
			var controllerName = typeof(T).Name;
			foreach (var controllerRunner in controllerChildren.GetChildrenRunners(default))
			{
				if (controllerRunner.PathInTree.Contains(controllerName) && controllerRunner.IsActive())
					return true;
			}
			return false;
		}
		
		public static bool IsActive(this IControllerRunnerBase controllerRunner)
		{
			return controllerRunner.ControllerStatus is ControllerStatus.Executing or ControllerStatus.Started;
		}
	}
}
