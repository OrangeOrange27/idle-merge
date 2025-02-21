using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using Package.ControllerTree;

namespace Common.EntryPoint.Initialize
{
    public class InitializeGameBeforeAuthController : ISimpleController
    {
        private readonly IEnumerable<IBeforeAuthInitialize> _beforeAuthInitializes;

        public InitializeGameBeforeAuthController(IEnumerable<IBeforeAuthInitialize> beforeAuthInitializes)
        {
            _beforeAuthInitializes = beforeAuthInitializes;
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
            await UniTask.WhenAll(_beforeAuthInitializes.Select(init => init.InitializeBeforeAuth()));
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