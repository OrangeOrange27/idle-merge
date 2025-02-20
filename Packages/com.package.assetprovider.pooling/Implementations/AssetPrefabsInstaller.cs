using Package.AssetProvider.Pooling.Implementations;
using Package.AssetProvider.Pooling.Infrastructure;
using Package.Pooling.Installers;
using UnityEngine;
using VContainer;

namespace Package.AssetProvider.Pooling
{
	public static class AssetPrefabsInstaller
	{
		public static void RegisterAssetPool<T>(this IContainerBuilder builder) where T : Component
		{
			builder.RegisterPool<T>();
			builder.Register<IAssetPrefabsPool<T>, AssetPrefabsPool<T>>(Lifetime.Singleton);
		}
	}
}
