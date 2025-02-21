using System;

namespace Package.StateMachine
{
	public class StateMachineException : Exception
	{
		public StateMachineException(string message) : base(message)
		{
		}

		public StateMachineException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}