using System;
using Package.ControllersTree.Abstractions;

namespace Package.ControllersTree.Settings
{
    public interface IControllerSettings
    {
        /// <summary>
        /// This method call each time when controller creates and after using returns to pool
        /// Pay attention that runner are shared and reusing from object pool
        /// </summary>
        IControllerRunner<TPayload, TResult> GetRunner<TController, TPayload, TResult>(IControllerRunnerBase parentControllerRunner, Func<TController> factory)
            where TController : IControllerWithPayloadAndReturn<TPayload, TResult>;

        IControllerRunnerBase GetRunner(IControllerRunnerBase parentControllerRunner, Func<IBaseController> factory);
    }
}