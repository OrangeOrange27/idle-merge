using Package.ControllersTree.Abstractions;
using Package.StateMachine;
using VContainer;

namespace Package.ControllersTree.VContainer
{
	public static class ControllersTreeInstaller
	{
		public static void RegisterController<TAbstraction, TImplementation>(this IContainerBuilder builder) where TAbstraction : IBaseController where  TImplementation : TAbstraction
		{
			builder.Register<TAbstraction, TImplementation>(Lifetime.Transient).AsImplementedInterfaces();
		}
		
		public static void RegisterControllersTreePackage(this IContainerBuilder builder)
		{
			builder.RegisterController<StateMachineController>();
		}

		public static void RegisterController<T>(this IContainerBuilder builder) where T : IBaseController
		{
			builder.Register<T>(Lifetime.Transient).AsSelf().AsImplementedInterfaces();
		}
	}
}
