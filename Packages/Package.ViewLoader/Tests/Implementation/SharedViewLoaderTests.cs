using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Common.AssetProvider.Infrastructure;
using Common.AssetProvider.Pooling.Implementations;
using Common.AssetProvider.Pooling.Infrastructure;
using Common.AssetProvider.ViewLoader.Infrastructure;
using Common.AssetProvider.ViewLoader.Tests.Mock;
using Common.Pooling.Infrastructures;
using Common.Pooling.Installers;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Common.AssetProvider.ViewLoader.Tests.Implementation
{
	public class SharedViewLoaderTests : ZenjectUnitTestFixture
	{
		private const string TestAddressableKey = "ViewLoaderTestAddressable";

		[SetUp]
		public override void Setup()
		{
			base.Setup();
		}

		private void RebindAllWithSubstitute()
		{
			Container.UnbindAll();

			var assetPrefabsPool = Substitute.For<IAssetPrefabsPool<TestView>>();
			var poolReference = Substitute.For<IPoolReference<TestView>>();
			assetPrefabsPool.GetReference(
				Arg.Any<string>(),
				Arg.Any<int>(),
				Arg.Any<CancellationToken>()
			).Returns(UniTask.FromResult(poolReference));

			Container.Bind<IAssetPrefabsPool<TestView>>().FromInstance(assetPrefabsPool);
			Container.BindSubstitute<IObjectHolderAcrossControllersChildren<ITestView>>();
			Container.BindSharedViewLoader<TestView, ITestView>(TestAddressableKey);
			Container.Rebind<IViewLoader<ITestView>>().FromInstance(Substitute.For<IViewLoader<ITestView>>());
		}

		private void RebindAllWithMock()
		{
			Container.UnbindAll();
			DestroyObjectsStrategyInstaller.Install(Container);
			PoolingInstaller.Install(Container);

			Container.BindInterfacesTo<ViewLoaderAssetProviderMock>().AsTransient();
			Container.BindIFactory<IAssetProvider>().FromResolve();

			Container.Bind<IAssetPrefabsPool<TestView>>().To<AssetPrefabsPool<TestView>>().AsSingle();
			Container.BindSubstitute<IObjectHolderAcrossControllersChildren<ITestView>>();
			Container.BindSharedViewLoader<TestView, ITestView>(TestAddressableKey);
		}

		[UnityTest]
		public IEnumerator SharedViewLoaderPrewarm_ViewLoaderPrewarmCalled()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithSubstitute();
				var sharedViewLoader = Container.Resolve<ISharedViewLoader<ITestView>>();
				var viewLoader = Container.Resolve<IViewLoader<ITestView>>();
				var resources = Substitute.For<ICollection<IDisposable>>();

				await sharedViewLoader.Prewarm(resources, CancellationToken.None);

				await viewLoader.Received().Prewarm(Arg.Any<ICollection<IDisposable>>(), Arg.Any<CancellationToken>());
			});
		}

		[UnityTest]
		public IEnumerator SharedViewLoaderLoad_ViewLoaderLoadCalled()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithSubstitute();
				var sharedViewLoader = Container.Resolve<ISharedViewLoader<ITestView>>();
				var viewLoader = Container.Resolve<IViewLoader<ITestView>>();
				var resources = Substitute.For<ICollection<IDisposable>>();
				var subscriptions = Substitute.For<ICollection<IDisposable>>();

				await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, null);

				await viewLoader.Received().Load(resources, Arg.Any<CancellationToken>(), Arg.Any<Transform>());
				subscriptions.Received().Add(Arg.Any<IDisposable>());
			});
		}

		[UnityTest]
		public IEnumerator SharedViewLoaderLoad_SecondLoadAfterResourcesDispose_DifferentInstanceReturned()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithMock();
				var destroyGameObjectsStrategy = Container.Resolve<IDestroyObjectsStrategy>();
				var resources = new CompositeDisposable();
				var subscriptions = new CompositeDisposable();
				var sharedViewLoader = Container.Resolve<ISharedViewLoader<ITestView>>();
				var transform = Substitute.For<Transform>();

				try
				{
					var instance1 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);
					resources.Dispose();
					var instance2 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);

					instance1.Should().NotBe(instance2);
				}
				finally
				{
					subscriptions.Dispose();
					resources.Dispose();
					await destroyGameObjectsStrategy.Wait();
					await Resources.UnloadUnusedAssets().ToUniTask();
				}
			});
		}

		[UnityTest]
		public IEnumerator SharedViewLoaderLoad_SecondLoadAfterSubscriptionsDispose_DifferentInstanceReturned()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithMock();
				var destroyGameObjectsStrategy = Container.Resolve<IDestroyObjectsStrategy>();
				var resources = new CompositeDisposable();
				var subscriptions = new CompositeDisposable();
				var sharedViewLoader = Container.Resolve<ISharedViewLoader<ITestView>>();
				var transform = Substitute.For<Transform>();

				try
				{
					var instance1 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);
					subscriptions.Dispose();
					var instance2 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);

					instance1.Should().NotBe(instance2);
				}
				finally
				{
					subscriptions.Dispose();
					resources.Dispose();
					await destroyGameObjectsStrategy.Wait();
					await Resources.UnloadUnusedAssets().ToUniTask();
				}
			});
		}
		[UnityTest]
		public IEnumerator SharedViewLoaderLoad_SecondLoadAfterSubscriptionsAndResourcesDispose_DifferentInstanceReturned()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithMock();
				var destroyGameObjectsStrategy = Container.Resolve<IDestroyObjectsStrategy>();
				var resources = new CompositeDisposable();
				var subscriptions = new CompositeDisposable();
				var sharedViewLoader = Container.Resolve<ISharedViewLoader<ITestView>>();
				var transform = Substitute.For<Transform>();

				try
				{
					var instance1 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);
					subscriptions.Dispose();
					var instance2 = await sharedViewLoader.Load(resources, subscriptions, CancellationToken.None, transform);
					resources.Dispose();

					instance1.Should().NotBe(instance2);
				}
				finally
				{
					subscriptions.Dispose();
					resources.Dispose();
					await destroyGameObjectsStrategy.Wait();
					await Resources.UnloadUnusedAssets().ToUniTask();
				}
			});
		}
	}
}