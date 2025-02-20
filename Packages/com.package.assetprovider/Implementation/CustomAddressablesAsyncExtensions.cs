using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AssetProvider.Implementation
{
	public static class CustomAddressablesAsyncExtensions
	{
        /// <summary>
        /// Duplicate Cysharp.Threading.Tasks.AddressablesAsyncExtensions.ToUniTask
        /// With adding progress as DownloadStatus to have progress per download size
        /// </summary>
		public static UniTask ToUniTask(this AsyncOperationHandle handle, IProgress<DownloadStatus> progress = null, PlayerLoopTiming timing = PlayerLoopTiming.Update, int updateProgresEveryNFrames = 30, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested) return UniTask.FromCanceled(cancellationToken);

			if (!handle.IsValid())
			{
				// autoReleaseHandle:true handle is invalid(immediately internal handle == null) so return completed.
				return UniTask.CompletedTask;
			}

			if (handle.IsDone)
			{
				if (handle.Status == AsyncOperationStatus.Failed)
				{
					return UniTask.FromException(handle.OperationException);
				}
				return UniTask.CompletedTask;
			}

			return new UniTask(CustomAsyncOperationHandleConfiguredSource.Create(handle, timing, progress, updateProgresEveryNFrames, cancellationToken, out var token), token);
		}

        private sealed class CustomAsyncOperationHandleConfiguredSource : IUniTaskSource, IPlayerLoopItem, ITaskPoolNode<CustomAsyncOperationHandleConfiguredSource>
        {
            static TaskPool<CustomAsyncOperationHandleConfiguredSource> pool;
            CustomAsyncOperationHandleConfiguredSource _nextNode;
            public ref CustomAsyncOperationHandleConfiguredSource NextNode => ref _nextNode;

            static CustomAsyncOperationHandleConfiguredSource()
            {
                TaskPool.RegisterSizeGetter(typeof(CustomAsyncOperationHandleConfiguredSource), () => pool.Size);
            }

            readonly Action<AsyncOperationHandle> continuationAction;
            AsyncOperationHandle handle;
            CancellationToken cancellationToken;
            IProgress<DownloadStatus> progress;
            bool completed;
            private int updateProgresEveryNFrames;

            UniTaskCompletionSourceCore<AsyncUnit> core;

            private CustomAsyncOperationHandleConfiguredSource()
            {
                continuationAction = Continuation;
            }

            public static IUniTaskSource Create(AsyncOperationHandle handle, PlayerLoopTiming timing, IProgress<DownloadStatus> progress, int updateProgresEveryNFrames, CancellationToken cancellationToken, out short token)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return AutoResetUniTaskCompletionSource.CreateFromCanceled(cancellationToken, out token);
                }

                if (!pool.TryPop(out var result))
                {
                    result = new CustomAsyncOperationHandleConfiguredSource();
                }

                result.handle = handle;
                result.progress = progress;
                result.cancellationToken = cancellationToken;
                result.completed = false;
                result.updateProgresEveryNFrames = updateProgresEveryNFrames;

                TaskTracker.TrackActiveTask(result, 3);

                PlayerLoopHelper.AddAction(timing, result);

                handle.Completed += result.continuationAction;

                token = result.core.Version;
                return result;
            }

            void Continuation(AsyncOperationHandle _)
            {
                handle.Completed -= continuationAction;

                if (completed)
                {
                    TryReturn();
                }
                else
                {
                    completed = true;
                    if (cancellationToken.IsCancellationRequested)
                    {
                        core.TrySetCanceled(cancellationToken);
                    }
                    else if (handle.Status == AsyncOperationStatus.Failed)
                    {
                        core.TrySetException(handle.OperationException);
                    }
                    else
                    {
                        core.TrySetResult(AsyncUnit.Default);
                    }
                }
            }

            public void GetResult(short token)
            {
                core.GetResult(token);
            }

            public UniTaskStatus GetStatus(short token)
            {
                return core.GetStatus(token);
            }

            public UniTaskStatus UnsafeGetStatus()
            {
                return core.UnsafeGetStatus();
            }

            public void OnCompleted(Action<object> continuation, object state, short token)
            {
                core.OnCompleted(continuation, state, token);
            }

            public bool MoveNext()
            {
                if (completed)
                {
                    TryReturn();
                    return false;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    completed = true;
                    core.TrySetCanceled(cancellationToken);
                    return false;
                }

                if (progress != null && handle.IsValid() && Time.frameCount % updateProgresEveryNFrames == 0)
                {
                    progress.Report(handle.GetDownloadStatus());
                }

                return true;
            }

            bool TryReturn()
            {
                TaskTracker.RemoveTracking(this);
                core.Reset();
                handle = default;
                progress = default;
                cancellationToken = default;
                return pool.TryPush(this);
            }
        }
	}
}
