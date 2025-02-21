using System.Collections.Generic;
using Package.StateMachine.Decorators;

namespace Package.StateMachine
{
	public interface IStateMachineInternalApi
	{
		void AddHooks(IStateMachineHooks instanceToAdd);
		void ChangeActiveState(IStateMachineInstruction stateMachineInstruction);
		void AddStateAsNext(IStateMachineInstruction stateMachineInstruction);
		List<IStateMachineInstruction> GetStateMachineInstructionsInStack();
		IStateMachineInstruction GetCurrentInstruction();
	}
}
