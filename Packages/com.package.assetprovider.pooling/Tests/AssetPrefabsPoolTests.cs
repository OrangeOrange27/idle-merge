using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using Common.AssetProvider.Pooling.Exceptions;
using Common.AssetProvider.Pooling.Implementations;
using Common.AssetProvider.Pooling.Infrastructure;
using Common.AssetProvider.Pooling.Tests.Data;
using Common.AssetProvider.Pooling.Tests.Extensions;
using Common.AssetProvider.Pooling.Tests.Mocks;
using Common.DestroyObjectsStrategies.Infrastructure;
using Common.DestroyObjectsStrategies.Installers;
using Common.Pooling.Installers;
using Cysharp.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using Tools.TestTools;
using UnityEngine;
using UnityEngine.TestTools;

namespace Common.AssetProvider.Pooling.Tests
{
	public class AssetPrefabsPoolTests : ZenjectUnitTestFixtureAsync
	{
		public override UniTask Setup()
		{
			DestroyObjectsStrategyInstaller.Install(Container);
			PoolingInstaller.Install(Container);
			AssetsPoolingInstaller.Install(Container);
			Container.BindAssetProviderMock();

			return base.Setup();
		}

		public override UniTask Teardown()
		{
			var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

			if (instanceCounter.NotDisposedCount > 0)
			{
				throw new InvalidOperationException("Tests should not leave footprint.");
			}

			return base.Teardown();
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsCreatedWhenMakingFirstReference([Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirstCase, [Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecondCase)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				instanceCounter.NotDisposedCount.Should().Be(0);

				using var disposables = new CompositeDisposable();

				await assetsPrefabsPool.MakeReferenceForTest(disposables, getReferenceApproachFirstCase);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_ExistentInstanceIsReusedWhenTryToMakeANewReference([Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirstCase,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecondCase)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				instanceCounter.NotDisposedCount.Should().Be(0);

				using var disposables = new CompositeDisposable();

				await assetsPrefabsPool.MakeReferenceForTest(disposables, getReferenceApproachFirstCase);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);

				await assetsPrefabsPool.MakeReferenceForTest(disposables, getReferenceApproachSecondCase);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsDisposedWhenUniqueReferenceIsDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				{
					using var disposables = new CompositeDisposable();

					await assetsPrefabsPool.MakeReferenceForTest(disposables, getReferenceApproach);

					instanceCounter.NotDisposedCount.Should().Be(1);
				}

				instanceCounter.NotDisposedCount.Should().Be(0);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsDisposedWhenLastReferenceIsDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				{
					using var firstReferencesDisposables = new CompositeDisposable();

					await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);

					instanceCounter.NotDisposedCount.Should().Be(1);

					{
						using var secondReferencesDisposables = new CompositeDisposable();

						await assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond);

						instanceCounter.NotDisposedCount.Should().Be(1);
					}

					instanceCounter.NotDisposedCount.Should().Be(1);
				}

				instanceCounter.NotDisposedCount.Should().Be(0);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsNotDisposedWhenDisposeReferenceButThereIsPreviouslyCreatedReferences(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();

				{
					using var secondReferencesDisposables = new CompositeDisposable();

					await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);
					await assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond);
				}

				instanceCounter.NotDisposedCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsNotDisposedWhenDisposeReferenceButThereIsNewlyCreatedReferences(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var secondReferencesDisposables = new CompositeDisposable();

				{
					using var firstReferencesDisposables = new CompositeDisposable();

					await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);
					await assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond);
				}

				instanceCounter.NotDisposedCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator AssetProviderInstance_IsRecreatedWhenDisposeAllReferencesAndMakeANewOne(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachInitial,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachRecreation)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				{
					using var firstReferencesDisposables = new CompositeDisposable();
					await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachInitial);
				}
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);

				using var secondReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachRecreation);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(2);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncThrowsAnExceptionAndThereIsNoAReferenceBefore_TestThatExceptionIsNotMutedAndAssetProviderIsDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();

				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproach, AssetPrefabsPoolTestAddresablesKey.ViewThatCausesException)
						.AsTask()
				).Should().ThrowExactlyAsync<Exception>().WithMessage("LoadAsync was failed intentionally!");

				firstReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(0);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncRetrieveInconsistentPrefabAndThereIsNoAReferenceBefore_TestExceptionIsRaisedAndAssetProviderIsDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach,
			[Values(AssetPrefabsPoolTestAddresablesKey.ViewThatNull, AssetPrefabsPoolTestAddresablesKey.ViewWithoutComponent)]
			string key
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();

				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproach, key).AsTask()
				).Should().ThrowExactlyAsync<AssetsPrefabsPoolProviderException>();

				firstReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(0);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncIsCancelledAndThereIsNoAReferenceBefore_TestThatExceptionIsNotMutedAndAssetProviderIsDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();
				using var cts = new CancellationTokenSource();
				cts.Cancel();

				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproach, AssetPrefabsPoolTestAddresablesKey.NormalView, cts.Token)
						.AsTask()
				).Should().ThrowExactlyAsync<OperationCanceledException>();

				firstReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(0);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncThrowsAnExceptionAndThereIsReference_TestThatExceptionIsNotMutedAndAssetProviderIsNotDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);

				using var secondReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool
						.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond, AssetPrefabsPoolTestAddresablesKey.ViewThatCausesException).AsTask()
				).Should().ThrowExactlyAsync<Exception>().WithMessage("LoadAsync was failed intentionally!");

				secondReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncIsCancelledAndThereAndThereIsReference_TestThatExceptionIsNotMutedAndAssetProviderIsNotDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);

				using var cts = new CancellationTokenSource();
				cts.Cancel();

				using var secondReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond, AssetPrefabsPoolTestAddresablesKey.NormalView, cts.Token)
						.AsTask()
				).Should().ThrowExactlyAsync<OperationCanceledException>();

				secondReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator LoadAsyncRetrievesInconsistentPrefabAndThereIsReference_TestThatExceptionIsRaisedAndAssetProviderIsNotDisposed(
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithGetItem,
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproachSecond,
			[Values(AssetPrefabsPoolTestAddresablesKey.ViewThatNull, AssetPrefabsPoolTestAddresablesKey.ViewWithoutComponent)]
			string key
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var firstReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.MakeReferenceForTest(firstReferencesDisposables, getReferenceApproachFirst);

				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);

				using var secondReferencesDisposables = new CompositeDisposable();
				await assetsPrefabsPool.Awaiting(
					pool => assetsPrefabsPool.MakeReferenceForTest(secondReferencesDisposables, getReferenceApproachSecond, key).AsTask()
				).Should().ThrowExactlyAsync<AssetsPrefabsPoolProviderException>();

				secondReferencesDisposables.Should().BeEmpty();
				instanceCounter.NotDisposedCount.Should().Be(1);
				instanceCounter.TotalCreatedSoFarCount.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator Prewarm_ReferenceIsAddedToDisposablesList()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var disposables = new CompositeDisposable();
				await assetsPrefabsPool.Prewarm(AssetPrefabsPoolTestAddresablesKey.NormalView, 1, disposables, CancellationToken.None);

				disposables.Should().NotBeEmpty();
			});
		}

		[UnityTest]
		public IEnumerator Get_ReferenceIsAddedToDisposablesList()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var disposables = new CompositeDisposable();
				await assetsPrefabsPool.Get(AssetPrefabsPoolTestAddresablesKey.NormalView, disposables, CancellationToken.None);

				disposables.Should().NotBeEmpty();
			});
		}

		[UnityTest]
		public IEnumerator ItemInstances_GetReferenceOrPrewarm_CreateExactAmountOfInstancesInHierarchy([Values(
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach,
			[Values(0, 1, 5)] int size
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var _ = await assetsPrefabsPool.GetReferenceForTest(getReferenceApproach, size: size);

				AmountOfViewsInPool.Should().Be(size);
			});
		}

		[UnityTest]
		public IEnumerator ItemInstances_GetReferenceOrPrewarm_ConsecutiveCallsWithWithNotExceedingSizeDoNotCreateNewInstancesInPool([Values(
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceFirst,
			[Values(
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceAdditional,
			[Values(1, 5)] int size)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var firstReference = await assetsPrefabsPool.GetReferenceForTest(getReferenceFirst, size: size);

				AmountOfViewsInPool.Should().Be(size);

				for (var nextSize = 1; nextSize <= size; nextSize++)
				{
					using var _ = await assetsPrefabsPool.GetReferenceForTest(getReferenceAdditional, size: nextSize);

					AmountOfViewsInPool.Should().Be(size);
				}

				AmountOfViewsInPool.Should().Be(size);
			});
		}

		[UnityTest]
		public IEnumerator Reference_AllPoolItemsHaveBeenTaken_AndGetIsInvoked_PoolCouldGrow([Values(0, 5)] int initialSize)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, initialSize);
				using var referencesDisposables = new CompositeDisposable();

				for (var i = 0; i < initialSize; i++)
				{
					poolReference.GetForTest(referencesDisposables);
				}

				var item1 = poolReference.GetForTest(referencesDisposables);
				item1.Should().NotBeNull();
				AmountOfViewsInPool.Should().Be(initialSize + 1);

				var item2 = poolReference.GetForTest(referencesDisposables);
				item2.Should().NotBeNull();
				AmountOfViewsInPool.Should().Be(initialSize + 2);
			});
		}

		[UnityTest]
		public IEnumerator Reference_ObjectsInPoolAreDestroyedWhenDisposeAllPoolReferences()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var destroyGameObjectsStrategy = Container.Resolve<IDestroyObjectsStrategy>();
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				{
					using var _ = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 2);
					AmountOfViewsInPool.Should().Be(2);
				}

				await Resources.UnloadUnusedAssets().ToUniTask();
				await destroyGameObjectsStrategy.Wait();

				AmountOnViewsTotal.Should().Be(0);
			});
		}

		[UnityTest]
		public IEnumerator Reference_GotFromPoolObjectsAlsoDestroyedEventIfThemAreNotReturned()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				AssetPrefabsPoolTestView view;

				{
					using var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 2);

					view = poolReference.Get();
					view.Should().NotBeNull();

					AmountOfViewsInPool.Should().Be(2);
				}

				await Resources.UnloadUnusedAssets().ToUniTask();

				(view == null).Should().BeTrue();
				AmountOnViewsTotal.Should().Be(0);
			});
		}

		[UnityTest]
		public IEnumerator Reference_ReturnedObjectsShouldBeRecycled(
			[Values(
				GetReferenceApproach.GettingReferenceWithPrewarm,
				GetReferenceApproach.GettingReferenceWithGetReference
			)]
			GetReferenceApproach getReferenceApproach
		)
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);
				using var referencesDisposables = new CompositeDisposable();

				AmountOfViewsInPool.Should().Be(1);

				var gotItem1 = poolReference.Get();
				poolReference.Return(gotItem1);

				AmountOfViewsInPool.Should().Be(1);

				var gotItem2 = poolReference.GetForTest(referencesDisposables);
				gotItem2.Should().Be(gotItem1);

				AmountOfViewsInPool.Should().Be(1);
			});
		}

		[UnityTest]
		public IEnumerator Reference_Get_DisposedPoolReference_Throws()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);
				poolReference.Dispose();

				poolReference.Invoking(reference => reference.Get()).Should().Throw<ObjectDisposedException>();
			});
		}

		[UnityTest]
		public IEnumerator Reference_Return_WhenDisposedPoolReference_Throws()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);
				var view = poolReference.Get();
				poolReference.Dispose();

				poolReference.Invoking(reference => reference.Return(view)).Should().Throw<ObjectDisposedException>();
			});
		}

		[UnityTest]
		public IEnumerator Reference_Return_PassingObjectIsNull_IgnoresAndNextGetRetrievesCorrectObject()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

				using var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);
				var view1 = poolReference.Get();

				poolReference.Invoking(reference => reference.Return(null)).Should().NotThrow();

				var view2 = poolReference.Get();

				view1.Should().NotBe(view2);
			});
		}

		[UnityTest]
		public IEnumerator Reference_DoubleDispose_SecondDisposeIsOkAndDidNotDisposeAssetProvider()
		{
			return UniTask.ToCoroutine(async () =>
			{
				var assetsPrefabsPool = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
				var instanceCounter = Container.Resolve<TestAssetsProviderInstanceCounter>();

				using var poolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);

				using var secondPoolReference = await assetsPrefabsPool.GetReference(AssetPrefabsPoolTestAddresablesKey.NormalView, 1);

				instanceCounter.NotDisposedCount.Should().Be(1);

				poolReference.Dispose();

				instanceCounter.NotDisposedCount.Should().Be(1);

				poolReference.Invoking(reference => reference.Dispose()).Should().NotThrow();

				instanceCounter.NotDisposedCount.Should().Be(1);

				secondPoolReference.Dispose();

				instanceCounter.NotDisposedCount.Should().Be(0);
			});
		}

		private static int AmountOnViewsTotal => Resources.FindObjectsOfTypeAll<AssetPrefabsPoolTestView>().Length;

		private int AmountOfViewsInPool
		{
			get
			{
				var amountOfViewsInHierarchy = AmountOnViewsTotal;
				var amountOfViewsInAssetProvider = Container.Resolve<TestAssetsProviderInstanceCounter>().ViewPrefabsLoadedCount;

				return amountOfViewsInHierarchy - amountOfViewsInAssetProvider;
			}
		}

		[Test]
		public void AssetsPrefabPoolIsSingle()
		{
			var firstResolve = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();
			var secondResolve = Container.Resolve<IAssetPrefabsPool<AssetPrefabsPoolTestView>>();

			firstResolve.Should().Be(secondResolve);
		}
	}
}