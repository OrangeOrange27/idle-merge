using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.Utils
{
    public static class Helpers
    {
        public static async UniTask WaitForPlayerInput(CancellationToken cancellationToken)
        {
#if UNITY_EDITOR
            await UniTask.WaitUntil(() => Input.GetMouseButtonDown(0), cancellationToken: cancellationToken);
#else
            await UniTask.WaitUntil(() => Input.touchCount > 0, cancellationToken: cancellationToken);
#endif
        }

        public static CancellationToken Combine(this CancellationToken token1, CancellationToken token2)
        {
            return CancellationTokenSource.CreateLinkedTokenSource(token1, token2).Token;
        }
    }
}