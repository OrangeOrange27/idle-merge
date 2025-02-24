using System;
using Package.AssetProvider.Pooling;
using Package.AssetProvider.ViewLoader.Implementation;
using Package.AssetProvider.ViewLoader.Infrastructure;
using UnityEngine;
using VContainer;

namespace Package.AssetProvider.ViewLoader.VContainer
{
	public static class ViewLoaderDiContainerExtensions
	{
		public static void RegisterViewLoader<TViewPrefab, TViewInterface>(this IContainerBuilder builder,
			string addressablesKey)
			where TViewPrefab : Component, TViewInterface where TViewInterface : class
		{
			builder.RegisterViewLoader<TViewPrefab, TViewInterface, int>(_ => addressablesKey);
		}

		public static void RegisterViewLoader<TViewPrefab, TViewInterface, TKey>(this IContainerBuilder builder,
			Func<TKey, string> convertKeyToAddressablesKey)
			where TViewPrefab : Component, TViewInterface where TViewInterface : class
		{
			builder.RegisterAssetPool<TViewPrefab>();
			builder.Register<ViewLoader<TViewPrefab, TViewInterface, TKey>>(Lifetime.Singleton)
				.WithParameter(convertKeyToAddressablesKey).AsImplementedInterfaces();
		}

		public static void RegisterSharedViewLoader<TViewPrefab, TViewInterface>(this IContainerBuilder builder,
			string addressablesKey)
			where TViewPrefab : Component, TViewInterface where TViewInterface : class
		{
			builder.RegisterViewLoader<TViewPrefab, TViewInterface>(addressablesKey);
			builder.Register<ISharedViewLoader<TViewInterface>, SharedViewLoader<TViewInterface, int>>(Lifetime.Singleton);
			builder.RegisterFactory<TViewInterface>(resolver => () => resolver.Resolve<ISharedViewLoader<TViewInterface>>().CachedView, Lifetime.Transient);
		}
		
		public static void RegisterSharedViewLoader<TViewPrefab, TViewInterface, TKey>(this IContainerBuilder builder,
			Func<TKey, string> convertKeyToAddressablesKey)
			where TViewPrefab : Component, TViewInterface where TViewInterface : class
		{
			builder.RegisterViewLoader<TViewPrefab, TViewInterface, TKey>(convertKeyToAddressablesKey);
			builder.Register<ISharedViewLoader<TViewInterface, TKey>, SharedViewLoader<TViewInterface, TKey>>(Lifetime.Singleton);
			builder.RegisterFactory<TViewInterface>(resolver => () => resolver.Resolve<ISharedViewLoader<TViewInterface>>().CachedView, Lifetime.Transient);
		}
	}
}
