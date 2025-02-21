using System;
using Cysharp.Threading.Tasks;

namespace Package.StateMachine.StateMachineErrorHandler
{
	public class DefaultStateMachineErrorHandler : IStateMachineErrorHandler
	{
		public IStateMachineInstruction ProcessException(Exception exception)
		{
			if (!exception.GetBaseException().IsOperationCanceledException())
			{
				throw exception;
			}
			
			return StateMachineInstruction.GoBack;
		}

		public void ProcessExceptionAtStopOrDispsoe(Exception exception)
		{
			//don't throw exception to not interupt next states working. Exception should be logged by LoggerControllerRunner
		}
	}
}
