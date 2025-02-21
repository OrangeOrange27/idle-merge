using Package.ControllersTree.Abstractions;

namespace Package.StateMachine
{
	public interface IStateController<in TPayload> : IControllerWithPayloadAndReturn<TPayload, IStateMachineInstruction>
	{
	}
	
	public interface IStateController : IStateController<EmptyPayloadType>
	{
	}
}