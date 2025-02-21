using Common.ApplicationLifecycleNotifier.Infrastructure;
using UnityEngine;
using VContainer;

namespace Common.ApplicationLifecycleNotifier
{
	public static class ApplicationLifecycleNotifierInstaller
	{
		public static void RegisterApplicationLifecycleNotifier(this IContainerBuilder builder)
		{
			var applicationLifecycleNotifier = new GameObject("ApplicationLifecycleNotifier")
				.AddComponent<ApplicationLifecycleNotifier>();
			Object.DontDestroyOnLoad(applicationLifecycleNotifier.gameObject);
			builder.RegisterInstance<IApplicationLifecycleNotifier>(applicationLifecycleNotifier);
		}
	}
}
