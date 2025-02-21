using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Implementations;
using Package.ControllersTree.Settings;

namespace Package.ControllersTree
{
    public static class ControllersTreeBootstrap
    {
        public static IControllerRunner<TPayload, TResult> Create<TPayload, TResult>(
            IControllerWithPayloadAndReturn<TPayload, TResult> initialController,
            IControllerSettings controllerSettings = null)
        {
            var controllerChildren = new ControllerChildren(null, controllerSettings ?? new DefaultControllerSettings());

            var runner =
                controllerChildren.Create<RootController<TPayload, TResult>, TPayload, TResult>(() => new RootController<TPayload, TResult>(initialController));
            return runner;
        }
    }
}