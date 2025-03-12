using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.Logger.Abstraction;
using UnityEngine;
using UnityEngine.Assertions;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Utils.Extensions
{
	public enum AnimCancellationType
	{
		Stop,
		Reset,
		Throw,
		ForceToEnd,
		Continue
	}

	public enum AnimationEndWaitingType
	{
		Delay,
		NormalizedTimeCheck
	}

	public static class AnimatorExtensions
	{
		private static readonly ILogger Logger = LogManager.GetLogger(nameof(AnimatorExtensions));

		public static void PlayClipOn(this Animator targetAnimator,
			string clipName, MonoBehaviour parentBehavior, Action endCallback)
		{
			if (parentBehavior == null)
			{
				Logger.ZLogError($"Can't play animation clip. {nameof(parentBehavior)} is null");
				return;
			}

			if (CanPlayAnimationClip(targetAnimator, clipName))
				parentBehavior.StartCoroutine(PlayClipCoroutine(targetAnimator, clipName, endCallback));
		}

		public static async UniTask PlayClipAsync(this Animator targetAnimator, string clipName)
		{
			if (!CanPlayAnimationClip(targetAnimator, clipName))
				return;

			targetAnimator.Play(clipName);

			await UniTask.WaitUntil((() => targetAnimator.GetCurrentAnimatorStateInfo(0).IsName(clipName)));

			var waitTime = targetAnimator.GetCurrentAnimatorStateInfo(0).length;

			await UniTask.WaitForSeconds(waitTime);
		}

		public static async UniTask WaitForClipEnd(this Animator targetAnimator, CancellationToken cancellationToken)
		{
			await UniTask.WaitUntil(ClipEnded, cancellationToken: cancellationToken);
			return;

			bool ClipEnded()
			{
				if (targetAnimator == null)
				{
					Logger.ZLogWarning($"TargetAnimator was probably destroyed before awaited clip ended");
					return true;
				}

				var normalizedTime = targetAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
				var totalLength = targetAnimator.GetCurrentAnimatorClipInfo(0).Sum(clip => clip.clip.length);

				return normalizedTime > totalLength && !targetAnimator.IsInTransition(0);
			}
		}

		private static bool CanPlayAnimationClip(Animator targetAnimator, string clipName)
		{
			if (targetAnimator == null)
			{
				Logger.ZLogError($"Can't play animation clip. {nameof(targetAnimator)} is null");
				return false;
			}

			if (string.IsNullOrEmpty(clipName) || !IsClipExists(targetAnimator, clipName))
			{
				Logger.ZLogError($"Can't find animation clip {clipName} on animator");
				return false;
			}

			return true;
		}

		private static bool IsClipExists(this Animator targetAnimator, string clipName)
		{
			Assert.IsNotNull(targetAnimator);
			Assert.IsFalse(string.IsNullOrEmpty(clipName));
			var targetClip =
				targetAnimator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == clipName);
			return targetClip != null;
		}

		private static IEnumerator PlayClipCoroutine(Animator targetAnimator, string clipName, Action endCallback)
		{
			var targetClip =
				targetAnimator.runtimeAnimatorController.animationClips.FirstOrDefault(c => c.name == clipName);
			targetAnimator.Play(clipName);
			yield return new WaitForSecondsRealtime(targetClip.length);
			endCallback?.Invoke();
		}

		public static event Action<Animator, int> AnimationChainIterationThresholdReached;

		private static int[] AnimationChainIterationThresholds = new int[]
		{
			20,
			100,
			500
		};

		public static Cysharp.Threading.Tasks.UniTask PlayAsync(this Animator animator, int stateHash,
			CancellationToken token, AnimCancellationType
				animCancellationType = AnimCancellationType.Continue, bool optimizeOneFrameClip = false)
		{
			return PlayAsync(animator, stateHash, cancellationToken: token, animCancellationType: animCancellationType,
				optimizeOneFrameClip: optimizeOneFrameClip);
		}

		public static Cysharp.Threading.Tasks.UniTask PlayAsync(
			this Animator animator, string stateName, int layerIndex = 0, float startTime = 0f,
			CancellationToken cancellationToken = default, AnimCancellationType
				animCancellationType = AnimCancellationType.Continue, bool optimizeOneFrameClip = false)
		{
			return PlayAsync(animator, Animator.StringToHash(stateName), layerIndex, startTime, cancellationToken,
				animCancellationType, optimizeOneFrameClip);
		}

		public static async Cysharp.Threading.Tasks.UniTask PlayAsync(
			this Animator animator, int stateHash, int layerIndex = 0, float startTime = 0f,
			CancellationToken cancellationToken = default, AnimCancellationType
				animCancellationType = AnimCancellationType.Continue, bool optimizeOneFrameClip = false)
		{
			cancellationToken.ThrowIfCancellationRequested();

			animator.enabled = true;

			animator.Play(stateHash, layerIndex, startTime);

			var currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);

			while (!cancellationToken.IsCancellationRequested && currentState.shortNameHash != stateHash)
			{
				await Cysharp.Threading.Tasks.UniTask.Yield(cancellationToken).SuppressCancellationThrow();

				if (!cancellationToken.IsCancellationRequested)
				{
					currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
				}
			}

			if (optimizeOneFrameClip && IsOneFrameClip(animator, layerIndex))
			{
				animator.Play(stateHash, layerIndex, 1f);
			}
			else
			{
				while (!cancellationToken.IsCancellationRequested && currentState.normalizedTime < 1f)
				{
					await Cysharp.Threading.Tasks.UniTask.Yield(cancellationToken).SuppressCancellationThrow();

					if (!cancellationToken.IsCancellationRequested)
					{
						currentState = animator.GetCurrentAnimatorStateInfo(layerIndex);
					}
				}
			}

			if (cancellationToken.IsCancellationRequested)
			{
				if (animator != null)
				{
					CancelAnimation(animator, stateHash, layerIndex, animCancellationType);
				}

				cancellationToken.ThrowIfCancellationRequested();

				return;
			}

			var isEnabledAnimatorRequired = animator.GetCurrentAnimatorClipInfo(layerIndex)
				.Any(clipInfo => clipInfo.clip.isLooping);

			animator.enabled = isEnabledAnimatorRequired;
		}

		public static async Cysharp.Threading.Tasks.UniTask TriggerAsync(
			this Animator animator, string trigger, bool needToSampleFrame = false,
			CancellationToken cancellationToken = default,
			AnimCancellationType animCancellationType = AnimCancellationType.Continue,
			bool optimizeOneFrameClip = false,
			AnimationEndWaitingType animationEndWaitingType = AnimationEndWaitingType.Delay)
		{
			cancellationToken.ThrowIfCancellationRequested();

			var triggerHash = Animator.StringToHash(trigger);

			await animator.TriggerAsync(triggerHash, needToSampleFrame, cancellationToken, animCancellationType,
				optimizeOneFrameClip, animationEndWaitingType);
		}

		public static async Cysharp.Threading.Tasks.UniTask TriggerAsync(
			this Animator animator, int triggerHash, bool needToSampleFrame = false,
			CancellationToken cancellationToken = default,
			AnimCancellationType animCancellationType = AnimCancellationType.Continue,
			bool optimizeOneFrameClip = false,
			AnimationEndWaitingType animationEndWaitingType = AnimationEndWaitingType.Delay)
		{
			animator.enabled = true;

			animator.SetTrigger(triggerHash);

			if (needToSampleFrame)
			{
				animator.Update(0f);
			}

			await animator.FollowAnimationChain(cancellationToken: cancellationToken,
				optimizeOneFrameClip: optimizeOneFrameClip, animationEndWaitingType: animationEndWaitingType);

			if (cancellationToken.IsCancellationRequested)
			{
				if (animator != null)
				{
					var currentHashName = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
					CancelAnimation(animator, currentHashName, 0, animCancellationType);
				}

				cancellationToken.ThrowIfCancellationRequested();

				return;
			}

			if (animator != null)
			{
				var isEnabledAnimatorRequired = animator.GetCurrentAnimatorClipInfo(0)
					.Any(clipInfo => clipInfo.clip.isLooping);

				animator.enabled = isEnabledAnimatorRequired;
			}
		}

		public static async Cysharp.Threading.Tasks.UniTask SetIntAsync(
			this Animator animator, string paramName, int paramValue, bool needToSampleFrame = false,
			CancellationToken cancellationToken = default,
			AnimCancellationType animCancellationType = AnimCancellationType.Continue,
			bool optimizeOneFrameClip = false)
		{
			cancellationToken.ThrowIfCancellationRequested();

			animator.enabled = true;

			var paramNameHash = Animator.StringToHash(paramName);

			animator.SetInteger(paramNameHash, paramValue);

			if (needToSampleFrame)
			{
				animator.Update(0f);
			}

			await animator
				.FollowAnimationChain(cancellationToken: cancellationToken, optimizeOneFrameClip: optimizeOneFrameClip)
				.SuppressCancellationThrow();

			if (cancellationToken.IsCancellationRequested)
			{
				if (animator != null)
				{
					var currentHashName = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
					CancelAnimation(animator, currentHashName, 0, animCancellationType);
				}

				cancellationToken.ThrowIfCancellationRequested();

				return;
			}


			if (animator != null)
			{
				var isEnabledAnimatorRequired = animator.GetCurrentAnimatorClipInfo(0)
					.Any(clipInfo => clipInfo.clip.isLooping);

				animator.enabled = isEnabledAnimatorRequired;
			}
		}

		/// <summary>
		/// Get currently played animation and wait till it ends
		/// </summary>
		/// <param name="animator">Targeted animator</param>
		/// <param name="cancellationToken">Cancellation token to stop the task</param>
		/// <param name="animatorLayerIndex">
		/// Index of the layer,
		/// from which the animator state will be received
		/// </param>
		/// <param name="optimizeOneFrameClip"></param>
		/// <remarks>
		/// Uses UniTask's <see cref="PreLateUpdate" /> when skipping frames,
		/// to make sure that animator's state was updated,
		/// see <a href="https://docs.unity3d.com/Manual/ExecutionOrder.html">Unity's execution order</a> for context
		/// </remarks>
		public static async Cysharp.Threading.Tasks.UniTask WaitCurrentStateIsEndedAsync(
			this Animator animator, CancellationToken cancellationToken, int animatorLayerIndex = 0,
			bool optimizeOneFrameClip = false)
		{
			await Cysharp.Threading.Tasks.UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken);

			cancellationToken.ThrowIfCancellationRequested();

			if (animator != null)
			{
				var timeLeft = animator.GetCurrentStateTimeLeft(optimizeOneFrameClip, animatorLayerIndex);
				await Cysharp.Threading.Tasks.UniTask.Delay(
					TimeSpan.FromSeconds(timeLeft),
					false,
					PlayerLoopTiming.PostLateUpdate,
					cancellationToken);
				cancellationToken.ThrowIfCancellationRequested();
			}
		}

		private static bool IsOneFrameClip(Animator animator, int layerIndex)
		{
			if (animator.GetCurrentAnimatorClipInfoCount(layerIndex) == 0)
				return true;

			var currentClipInfo = animator.GetCurrentAnimatorClipInfo(layerIndex)[0];

			if (currentClipInfo.clip == null)
				return true;

			return Mathf.Approximately(currentClipInfo.clip.length, 0f);
		}

		private static float GetCurrentStateTimeLeft(this Animator animator, bool optimizeOneFrameClip,
			int layerIndex = 0)
		{
			if (optimizeOneFrameClip && IsOneFrameClip(animator, layerIndex))
			{
				return 0f;
			}

			var animatorStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
			var currentLoopNormalizedTime = Mathf.Abs(
				(float)(animatorStateInfo.normalizedTime - Math.Truncate(animatorStateInfo.normalizedTime)));

			return (1 - currentLoopNormalizedTime) * animatorStateInfo.length;
		}

		private static async Cysharp.Threading.Tasks.UniTask WaitClipAndTransitionAsync(
			this Animator animator, int animatorLayerIndex = 0,
			bool optimizeOneFrameClip = false, CancellationToken cancellationToken = default,
			AnimationEndWaitingType animationEndWaitingType = AnimationEndWaitingType.Delay)
		{
			if (animator != null)
			{
				if (animationEndWaitingType == AnimationEndWaitingType.NormalizedTimeCheck)
				{
					while (!cancellationToken.IsCancellationRequested &&
					       animator.GetCurrentAnimatorStateInfo(animatorLayerIndex).normalizedTime < 1f)
					{
						await Cysharp.Threading.Tasks.UniTask.Yield(cancellationToken);
					}
				}
				else
				{
					var timeLeft = animator.GetCurrentStateTimeLeft(optimizeOneFrameClip, animatorLayerIndex);

					if (!Mathf.Approximately(timeLeft, 0f))
					{
						await Cysharp.Threading.Tasks.UniTask.Delay(
							TimeSpan.FromSeconds(timeLeft),
							false,
							PlayerLoopTiming.PostLateUpdate,
							cancellationToken).SuppressCancellationThrow();
					}
					else
					{
						animator.Update(1f);
					}
				}

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				await Cysharp.Threading.Tasks.UniTask.WaitUntil(
					() => !IsAnimatorInTransition(animator, animatorLayerIndex),
					PlayerLoopTiming.PostLateUpdate,
					cancellationToken).SuppressCancellationThrow();
			}
		}

		private static async Cysharp.Threading.Tasks.UniTask FollowAnimationChain(this Animator animator,
			int layerIndex = 0, CancellationToken cancellationToken = default,
			bool optimizeOneFrameClip = false,
			AnimationEndWaitingType animationEndWaitingType = AnimationEndWaitingType.Delay)
		{
			var proceed = true;
			var iterations = 0;

			while (proceed)
			{
				iterations++;

				await Cysharp.Threading.Tasks.UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken)
					.SuppressCancellationThrow();

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				if (animator == null)
				{
					return;
				}

				if (AnimationChainIterationThresholds.Contains(iterations))
				{
					AnimationChainIterationThresholdReached?.Invoke(animator, iterations);
				}

				var currentClipNames = animator.GetCurrentAnimatorClipInfo(layerIndex)
					.Select(clipInfo => clipInfo.clip.name);

				await animator
					.WaitClipAndTransitionAsync(layerIndex, optimizeOneFrameClip, cancellationToken,
						animationEndWaitingType).SuppressCancellationThrow();

				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}

				await Cysharp.Threading.Tasks.UniTask.Yield(PlayerLoopTiming.PostLateUpdate, cancellationToken)
					.SuppressCancellationThrow();

				if (cancellationToken.IsCancellationRequested || animator == null)
				{
					return;
				}

				var nextClipNames = animator.GetCurrentAnimatorClipInfo(layerIndex)
					.Select(clipInfo => clipInfo.clip.name);

				proceed = currentClipNames.Except(nextClipNames).Any();
			}
		}

		private static bool IsAnimatorInTransition(Animator animator, int animatorLayerIndex)
		{
			if (animator == null)
			{
				return false;
			}

			return animator.IsInTransition(animatorLayerIndex);
		}

		private static void CancelAnimation(Animator animator, int stateHash, int layerIndex,
			AnimCancellationType animCancellationType)
		{
			switch (animCancellationType)
			{
				case AnimCancellationType.Stop:
					animator.enabled = false;

					break;
				case AnimCancellationType.ForceToEnd:
					animator.Play(stateHash, layerIndex, 1);

					break;

				case AnimCancellationType.Continue:
					animator.enabled = true;

					break;
				default:
					animator.enabled = false;

					break;
			}
		}

		internal static void AddAnimationThreshold(int threshold)
		{
			if (AnimationChainIterationThresholds.Contains(threshold))
			{
				return;
			}

			var thresholdsList = AnimationChainIterationThresholds.ToList();
			thresholdsList.Add(threshold);
			AnimationChainIterationThresholds = thresholdsList.ToArray();
		}
	}
}