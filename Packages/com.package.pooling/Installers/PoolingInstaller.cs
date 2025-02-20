using Package.Pooling.Factory;
using Package.Pooling.Implementations;
using Package.Pooling.Infrastructures;
using Package.Pooling.Strategies;
using UnityEngine;
using VContainer;

namespace Package.Pooling.Installers
{
	public static class PoolingInstaller
	{
		public static void RegisterDefaultPoolStrategy(this IContainerBuilder builder)
		{
			builder.Register<DefaultPoolStrategy>(Lifetime.Singleton).As<ICreatePoolStrategy, IGetPoolStrategy, IReturnPoolStrategy>();
		}

		public static void RegisterGameObjectPrefabsPool(this IContainerBuilder builder)
		{
			builder.Register<IPrefabsPool<GameObject>, PrefabsPoolByKey>(Lifetime.Singleton);
			builder.Register<PrefabsPoolGameObject>(Lifetime.Transient);

			builder.RegisterFactory<IPrefabsPool<GameObject>>(resolver => resolver.Resolve<PrefabsPoolGameObject>, Lifetime.Singleton);
		}

		public static void RegisterPool<T>(this IContainerBuilder builder) where T : Component
		{
			builder.Register<PrefabsPoolComponent<T>>(Lifetime.Singleton);
			builder.Register<IPrefabsPool<T>, PrefabsPoolComponent<T>>(Lifetime.Singleton);
		}
	}
}
