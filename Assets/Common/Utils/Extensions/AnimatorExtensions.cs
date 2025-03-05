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
	}
}