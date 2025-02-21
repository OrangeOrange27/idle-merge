using Package.ControllersTree.VContainer;
using VContainer;

namespace Common.TimeService
{
	public static class TimeServiceRegistration
	{
		public static void RegisterTimeService(this IContainerBuilder builder)
		{
			builder.Register<TimeService>(Lifetime.Singleton).AsImplementedInterfaces();
			builder.RegisterController<UpdateBootTimeController>();
		}
	}
}
