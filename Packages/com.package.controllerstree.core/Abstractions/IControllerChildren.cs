using System;
using System.Collections.Generic;

namespace Package.ControllersTree.Abstractions
{
    public interface IControllerChildren
    {
        IControllerRunner<TPayload, TResult> Create<TController, TPayload, TResult>(Func<TController> factory)
            where TController : IControllerWithPayloadAndReturn<TPayload, TResult>;

        IControllerRunnerBase Create(Func<IBaseController> factory);

        IEnumerable<IControllerRunnerBase> GetChildrenRunners(IBaseController controller);
    }
}