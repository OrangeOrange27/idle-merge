using System;

namespace Common.ApplicationLifecycleNotifier.Infrastructure
{
	public interface IApplicationLifecycleNotifier
	{
		event Action<bool> ApplicationPauses;
		event Action<bool> ApplicationFocuses;
		event Action ApplicationQuit;
	}
}
