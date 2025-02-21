using System;
using Cysharp.Threading.Tasks;

namespace Package.StateMachine.StateMachineErrorHandler
{
	public class RethrowStateMachineErrorHandler : IStateMachineErrorHandler
	{
		public IStateMachineInstruction ProcessException(Exception exception)
		{
			if (!exception.GetBaseException().IsOperationCanceledException()) throw exception;

			return StateMachineInstruction.GoBack;
		}

		public void ProcessExceptionAtStopOrDispsoe(Exception exception)
		{
			throw exception;
		}
	}
}
