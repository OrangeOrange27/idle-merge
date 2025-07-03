using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Common.UI
{
    public interface IView
    {
        event Action OnCloseButtonPressedEvent;
        
        UniTask ShowAsync(CancellationToken cancellationToken);
        UniTask HideAsync(CancellationToken cancellationToken);
    }
}
