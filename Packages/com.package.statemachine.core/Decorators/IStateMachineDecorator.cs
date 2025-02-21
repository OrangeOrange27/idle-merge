using Package.ControllersTree.Abstractions;

namespace Package.StateMachine.Decorators
{
	public interface IStateMachineDecorator : IControllerWithPayloadAndReturn<IStateMachineInternalApi, EmptyPayloadType>
	{
	}
}
