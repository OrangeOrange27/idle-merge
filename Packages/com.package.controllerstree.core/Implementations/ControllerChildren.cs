using System;
using System.Collections.Generic;
using System.Linq;
using Package.ControllersTree.Abstractions;
using Package.ControllersTree.Settings;

namespace Package.ControllersTree.Implementations
{
    public class ControllerChildren : IControllerChildren
    {
        private readonly IControllerRunnerBase _parentRunner;
        private readonly IControllerSettings _controllerSettings;
        private List<IControllerRunnerBase> _childRunners;

        private IList<IControllerRunnerBase> ChildRunners =>
            _childRunners ??= new List<IControllerRunnerBase>();

        public ControllerChildren(IControllerRunnerBase parentRunner, IControllerSettings controllerSettings)
        {
            _parentRunner = parentRunner;
            _controllerSettings = controllerSettings;
        }

        public IControllerRunner<TPayload, TResult> Create<TController, TPayload, TResult>(Func<TController> factory)
            where TController : IControllerWithPayloadAndReturn<TPayload, TResult>
        {
            if (factory == null)
                throw new ControllerException($"Factory of {typeof(TController).FullName} is null");
            //todo add pool
            var runner = _controllerSettings.GetRunner<TController, TPayload, TResult>(_parentRunner, factory);
            ChildRunners.Add(runner);
            return runner;
        }

        public IControllerRunnerBase Create(Func<IBaseController> factory)
        {
            if (factory == null)
                throw new ControllerException($"Factory of  is null");
            var runner = _controllerSettings.GetRunner(_parentRunner, factory);
            ChildRunners.Add(runner);
            return runner;        
        }

        public IEnumerable<IControllerRunnerBase> GetChildrenRunners(IBaseController controller)
        {
            return _childRunners == null ? Enumerable.Empty<IControllerRunnerBase>() : ChildRunners;
        }
    }
}