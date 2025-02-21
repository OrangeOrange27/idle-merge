using System;

namespace Package.StateMachine.StateMachineErrorHandler
{
	public interface IStateMachineErrorHandler
	{
		IStateMachineInstruction ProcessException(Exception exception);
		void ProcessExceptionAtStopOrDispsoe(Exception exception);
	}
}
