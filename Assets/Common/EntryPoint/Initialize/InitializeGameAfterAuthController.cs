using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllerTree;

namespace Common.EntryPoint.Initialize
{
    public class InitializeGameAfterAuthController : ISimpleController
    {
        private readonly IEnumerable<IAfterAuthInitialize> _afterAuthInitializes;

        public InitializeGameAfterAuthController(IEnumerable<IAfterAuthInitialize> afterAuthInitializes)
        {
            _afterAuthInitializes = afterAuthInitializes;
        }

        public UniTask OnInitialize(IControllerResources resources, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnStart(EmptyPayloadType payload, IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public async UniTask<EmptyPayloadType> Execute(IControllerResources resources, IControllerChildren controllerChildren, CancellationToken token)
        {
            await UniTask.WhenAll(_afterAuthInitializes.Select(init => init.InitializeAfterAuth()));
            return default;
        }

        public UniTask OnStop(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }

        public UniTask OnDispose(CancellationToken token)
        {
            return UniTask.CompletedTask;
        }
    }
}