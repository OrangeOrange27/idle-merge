using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Implementations
{
	public static class PrefabsPoolExtensions
	{
		public static IPoolReference<T> GetPoolReference<T>(
			this IPrefabsPool<T> pool, GameObject prefab, int size = 0) where T : Component =>
			pool.GetPoolReference(prefab.GetComponent<T>(), size);
	}
}