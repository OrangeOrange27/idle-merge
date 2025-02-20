using System;
using System.Collections.Generic;
using System.Linq;
using Package.Pooling.Infrastructures;
using UnityEngine;
using Component = UnityEngine.Component;

namespace Package.Pooling.Factory
{
	public class PrefabsPoolByKey : IPrefabsPool<GameObject>
	{
		private readonly Func<IPrefabsPool<GameObject>> _factoryFunc;

		private readonly Dictionary<UnityEngine.Object, IPrefabsPool<GameObject>> _prefabsPools = new();
		private Transform _root;
		private int _size;

		public PrefabsPoolByKey(Func<IPrefabsPool<GameObject>> factoryFunc)
		{
			_factoryFunc = factoryFunc;
		}

		public int FreeElementsInPool => _prefabsPools.First().Value.FreeElementsInPool;

		public Transform Root => _prefabsPools.First().Value.Root;

		public int Size => _prefabsPools.First().Value.Size;

		public IPoolReference<GameObject> GetPoolReference(GameObject prefab, int size = 0)
		{
			if (_prefabsPools.TryGetValue(prefab, out var pool))
			{
				return pool.GetPoolReference(prefab, size);
			}
			else
			{
				var newPool = _factoryFunc.Invoke();
				_prefabsPools.Add(prefab, newPool);
				return newPool.GetPoolReference(prefab, size);
			}
		}
	}
}
