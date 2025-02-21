using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Implementations;

namespace Package.ControllersTree.Abstractions
{
    public interface IControllerRunner<in TPayload, TResult> : IControllerRunnerBase,
        IControllerRunnerPayload<TPayload>, IControllerRunnerReturn<TResult>, IControllerRunnerAwaitableCallbacks, IControllerRunnerChildren
    {
    }

    public interface IControllerRunnerBase
    {
        ControllerStatus ControllerStatus { get; }
        UniTask Initialize(CancellationToken token);
        UniTask Stop(CancellationToken token);
        UniTask Dispose(CancellationToken token);
        string PathInTree { get; }
    }

    public interface IControllerRunnerPayload<in TPayload>
    {
        UniTask Start(TPayload payload, CancellationToken token);
    }

    public interface IControllerRunnerReturn<TResult>
    {
        UniTask<TResult> Execute(CancellationToken token);
    }
    
    public interface IControllerRunnerChildren
    {
        IControllerChildren GetControllerChildren();
    }
    
    public interface IControllerRunnerAwaitableCallbacks
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status">Supports only ControllerStatus.Stopped and ControllerStatus.Disposed.
        /// It wasn't suppoerted for all statuses to not make overhead for every controller run</param>
        /// <param name="token"></param>
        /// <returns></returns>
        UniTask AwaitControllerStatus(ControllerStatus status, CancellationToken token);
    }
}