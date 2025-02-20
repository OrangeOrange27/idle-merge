using Common.AssetProvider.Infrastructure;
using Common.AssetProvider.Pooling.Tests.Data;
using Common.AssetProvider.Pooling.Tests.Mocks;
using Zenject;

namespace Common.AssetProvider.Pooling.Tests.Extensions
{
	internal static class AssetsPrefabsPoolTestDIContainerExtensions
	{
		public static void BindAssetProviderMock(this DiContainer container)
		{
			container.Bind<TestAssetsProviderInstanceCounter>().AsSingle();
			container.BindInterfacesTo<AssetsPrefabsPoolTestsAssetProviderMock>().AsTransient();
			container.BindIFactory<IAssetProvider>().FromResolve();
		}
	}
}