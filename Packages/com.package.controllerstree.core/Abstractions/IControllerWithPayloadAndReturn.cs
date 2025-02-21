using System.Threading;
using Cysharp.Threading.Tasks;

namespace Package.ControllersTree.Abstractions
{
    public interface IControllerWithPayloadAndReturn<in TPayload, TResult> : IBaseController,
        IControllerPayload<TPayload>, IControllerResult<TResult>
    {
    }

    public interface IBaseController
    {
        UniTask OnInitialize(IControllerResources resources, CancellationToken token);
        UniTask OnStop(CancellationToken token);
        UniTask OnDispose(CancellationToken token);
    }

    public interface IControllerPayload<in TPayload>
    {
        UniTask OnStart(TPayload payload, IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token);
    }

    public interface IControllerResult<TResult>
    {
        UniTask<TResult> Execute(IControllerResources resources, IControllerChildren controllerChildren,
            CancellationToken token);
    }
}