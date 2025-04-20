using System;
using System.Collections.Generic;
using System.Text;
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

        public static string FormatTimer(TimeSpan time)
        {
            if (time.TotalSeconds <= 0)
                return "0s";

            var sb = new StringBuilder();

            if (time.Days > 0)
                sb.Append($"{time.Days}d");
            if (time.Hours > 0 || sb.Length > 0)
                sb.Append($"{time.Hours}h");
            if (time.Minutes > 0 || sb.Length > 0)
                sb.Append($"{time.Minutes}m");
            if (time.Seconds > 0 || sb.Length == 0)
                sb.Append($"{time.Seconds}s");

            return sb.ToString();
        }
    }
}