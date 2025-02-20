using System;
using System.Collections.Generic;
using System.Threading;
using Common.AssetProvider.Implementation;
using Common.AssetProvider.Pooling.Implementations;
using Common.AssetProvider.Pooling.Infrastructure;
using Common.AssetProvider.Pooling.Tests.Data;
using Common.Pooling.Infrastructures;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Common.AssetProvider.Pooling.Tests.Extensions
{
	internal static class AssetsPrefabsPoolTestExtensions
	{
		public static async UniTask MakeReferenceForTest<T>(this IAssetPrefabsPool<T> assetPrefabsPool, ICollection<IDisposable> disposables, GetReferenceApproach getReferenceApproach,
			string key = AssetPrefabsPoolTestAddresablesKey.NormalView, CancellationToken cancellationToken = default, int size = 1) where T : Component
		{
			switch (getReferenceApproach)
			{
				case GetReferenceApproach.GettingReferenceWithGetItem:
					for (var i = 0; i < size; ++i)
					{
						await assetPrefabsPool.Get(key, disposables, cancellationToken);
					}

					break;
				case GetReferenceApproach.GettingReferenceWithPrewarm:
					await assetPrefabsPool.Prewarm(key, size, disposables, cancellationToken);

					break;
				case GetReferenceApproach.GettingReferenceWithGetReference:
					disposables.Add(await assetPrefabsPool.GetReference(key, size, cancellationToken));

					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(getReferenceApproach), getReferenceApproach, null);
			}
		}

		public static async UniTask<IDisposable> GetReferenceForTest<T>(this IAssetPrefabsPool<T> assetPrefabsPool, GetReferenceApproach getReferenceApproach,
			string key = AssetPrefabsPoolTestAddresablesKey.NormalView, CancellationToken cancellationToken = default, int size = 1) where T : Component
		{
			var disposables = new CompositeDisposable();

			await MakeReferenceForTest(assetPrefabsPool, disposables, getReferenceApproach, key, cancellationToken, size);

			return disposables;
		}

		public static T GetForTest<T>(this IPoolReference<T> prefabsPoolReference, ICollection<IDisposable> disposables,
			Transform parent = null) where T : Component
		{
			var item = prefabsPoolReference.Get(parent);
			disposables.Add(new Disposable(() => prefabsPoolReference.Return(item)));
			return item;
		}
	}
}