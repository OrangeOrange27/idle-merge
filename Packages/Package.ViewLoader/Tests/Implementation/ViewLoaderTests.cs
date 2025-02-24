using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Common.AssetProvider.Infrastructure;
using Common.AssetProvider.Pooling.Implementations;
using Common.AssetProvider.Pooling.Infrastructure;
using Common.AssetProvider.ViewLoader.Infrastructure;
using Common.AssetProvider.ViewLoader.Tests.Mock;
using Common.AssetProvider.ViewLoader.Zenject;
using Common.DestroyObjectsStrategies.Infrastructure;
using Common.DestroyObjectsStrategies.Installers;
using Common.Pooling.Infrastructures;
using Common.Pooling.Installers;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;
using Tools.TestTools;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace Common.AssetProvider.ViewLoader.Tests.Implementation
{
	public class ViewLoaderTests : ZenjectUnitTestFixture
	{
		private const string TestAddressableKey = "ViewLoaderTestAddressable";
		private const string TestAddressableFirstKey = "ViewLoaderTestAddressableFirst";
		private const string TestAddressableSecondKey = "ViewLoaderTestAddressableSecond";
		private const string ContextConfigurationKey = "contextConfigurationKey";

		[SetUp]
		public override void Setup()
		{
			base.Setup();
		}

		private void RebindAllWithSubstitute()
		{
			Container.UnbindAll();

			BindSubstitute<TestView>();

			Container.BindViewLoader<TestView, ITestView>(TestAddressableKey);
		}

		private void RebindAllWithMock()
		{
			Container.UnbindAll();
			DestroyObjectsStrategyInstaller.Install(Container);
			PoolingInstaller.Install(Container);

			Container.BindInterfacesTo<ViewLoaderAssetProviderMock>().AsTransient();
			Container.BindIFactory<IAssetProvider>().FromResolve();

			Container.Bind<IAssetPrefabsPool<TestView>>().To<AssetPrefabsPool<TestView>>().AsSingle();

			Container.BindViewLoader<TestView, ITestView>(TestAddressableKey);
		}

		private void BindSubstitute<TView>() where TView : Component
		{
			var assetPrefabsPool = Substitute.For<IAssetPrefabsPool<TView>>();
			var poolReference = Substitute.For<IPoolReference<TView>>();
			assetPrefabsPool.GetReference(
				Arg.Any<string>(),
				Arg.Any<int>(),
				Arg.Any<CancellationToken>()
			).Returns(UniTask.FromResult(poolReference));

			Container.Bind<IAssetPrefabsPool<TView>>().FromInstance(assetPrefabsPool);
			Container.Bind<IPoolReference<TView>>().FromInstance(poolReference);
		}

		[UnityTest]
		public IEnumerator ViewLoaderPrewarm_PoolReferenceGotButInstanceNotGot()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithSubstitute();
				var viewLoader = Container.Resolve<IViewLoader<ITestView>>();
				var assetPrefabsPool = Container.Resolve<IAssetPrefabsPool<TestView>>();
				var poolReference = Container.Resolve<IPoolReference<TestView>>();
				var resources = Substitute.For<ICollection<IDisposable>>();

				await viewLoader.Prewarm(resources, CancellationToken.None);

				await assetPrefabsPool.Received().GetReference(
					TestAddressableKey,
					Arg.Any<int>(),
					Arg.Any<CancellationToken>());
				poolReference.DidNotReceive().Get(Arg.Any<Transform>());
			});
		}

		[UnityTest]
		public IEnumerator ViewLoaderLoad_PoolReferenceGotAndInstanceGot()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithSubstitute();
				await ValidateLoaderWithReceivedReference<TestView>(TestAddressableKey);
			});
		}

		[UnityTest]
		public IEnumerator ViewLoaderLoad_SecondLoad_DifferentInstanceReturned()
		{
			return UniTask.ToCoroutine(async () =>
			{
				RebindAllWithMock();
				var destroyGameObjectsStrategy = Container.Resolve<IDestroyObjectsStrategy>();
				var resources = new CompositeDisposable();
				var viewLoader = Container.Resolve<IViewLoader<ITestView>>();
				var transform = Substitute.For<Transform>();

				try
				{
					var instance1 = await viewLoader.Load(resources, CancellationToken.None, transform);
					var instance2 = await viewLoader.Load(resources, CancellationToken.None, transform);

					instance1.Should().NotBe(instance2);
				}
				finally
				{
					resources.Dispose();
					await destroyGameObjectsStrategy.Wait();
					await Resources.UnloadUnusedAssets().ToUniTask();
				}
			});
		}

		[UnityTest]
		public IEnumerator ViewLoaderLoad_PoolReferenceGotAndInstanceGot_WithWhenCondition()
		{
			return UniTask.ToCoroutine(async () =>
			{
				Container.UnbindAll();

				BindSubstitute<TestViewFirst>();
				BindSubstitute<TestViewSecond>();

				var contextConfigurationHolder = new Dictionary<string, string>();

				const string firstKey = "firstKey";
				const string secondKey = "secondKey";

				Container.BindViewLoaderWhen<TestViewFirst, ITestView>(TestAddressableFirstKey,
					context => contextConfigurationHolder[ContextConfigurationKey] == firstKey);
				Container.BindViewLoaderWhen<TestViewSecond, ITestView>(TestAddressableSecondKey,
					context => contextConfigurationHolder[ContextConfigurationKey] == secondKey);

				ChangeConfigurationContextHolder(contextConfigurationHolder, firstKey);
				await ValidateLoaderWithReceivedReference<TestViewFirst>(TestAddressableFirstKey);

				ChangeConfigurationContextHolder(contextConfigurationHolder, secondKey);
				await ValidateLoaderWithReceivedReference<TestViewSecond>(TestAddressableSecondKey);
			});
		}

		private async UniTask ValidateLoaderWithReceivedReference<TView>(string requiredAddressableKey) where TView : Component
		{
			var viewLoader = Container.Resolve<IViewLoader<ITestView>>();

			await viewLoader.Load(Substitute.For<ICollection<IDisposable>>(), CancellationToken.None, null);

			var assetPrefabsPoolFirst = Container.Resolve<IAssetPrefabsPool<TView>>();
			var poolReferenceFirst = Container.Resolve<IPoolReference<TView>>();
			await assetPrefabsPoolFirst.Received().GetReference(
				requiredAddressableKey,
				Arg.Any<int>(),
				Arg.Any<CancellationToken>());
			poolReferenceFirst.Received().Get(Arg.Any<Transform>());
		}

		private static void ChangeConfigurationContextHolder(Dictionary<string, string> contextConfigurationHolder, string configuration)
		{
			contextConfigurationHolder.Clear();
			contextConfigurationHolder.Add(ContextConfigurationKey, configuration);
		}
	}
}