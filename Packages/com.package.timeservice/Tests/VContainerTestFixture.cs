using NUnit.Framework;
using VContainer;

namespace Common.TimeService.Tests
{
	public abstract class VContainerTestFixture
	{
		protected IObjectResolver Resolver;

		[SetUp]
		public void Setup()
		{
			var containerBuilder = new ContainerBuilder();
			InstallDependencies(containerBuilder);
			Resolver = containerBuilder.Build();
		}

		protected abstract void InstallDependencies(IContainerBuilder containerBuilder);
	}
}
