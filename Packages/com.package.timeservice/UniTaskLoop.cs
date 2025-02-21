using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Package.TimeService
{
	public static class UniTaskLoop
	{
		public static UniTask AsyncLoop<T>(Func<UniTask> func, CancellationToken token,
			UniTaskCompletionSource<T> completionSource,
			PlayerLoopTiming loopTiming = PlayerLoopTiming.Update)
		{
			return AsyncLoop(func,
				() => token.IsCancellationRequested || completionSource.Task.Status != UniTaskStatus.Pending,
				loopTiming);
		}
		public static UniTask AsyncLoop(Func<UniTask> func, CancellationToken token,
			UniTaskCompletionSource completionSource,
			PlayerLoopTiming loopTiming = PlayerLoopTiming.Update)
		{
			return AsyncLoop(func,
				() => token.IsCancellationRequested || completionSource.Task.Status != UniTaskStatus.Pending,
				loopTiming);
		}

		public static UniTask AsyncLoop(Func<UniTask> func, CancellationToken token,
			PlayerLoopTiming loopTiming = PlayerLoopTiming.Update)
		{
			return AsyncLoop(func, () => token.IsCancellationRequested, loopTiming);
		}

		public static async UniTask AsyncLoop(Func<UniTask> func, Func<bool> stopUpdate,
			PlayerLoopTiming loopTiming = PlayerLoopTiming.Update)
		{
			while (!stopUpdate.Invoke())
			{
				await func.Invoke();
				await UniTask.Yield(loopTiming);
			}
		}
	}
}
