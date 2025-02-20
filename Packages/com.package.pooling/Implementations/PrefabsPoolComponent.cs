using System.Collections.Generic;
using System.Linq;
using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Implementations
{
	public sealed class PrefabsPoolComponent<T> : IPrefabsPool<T> where T : Component
	{
		private readonly IPrefabsPool<GameObject> _gameObjectPrefabsPool;

		public PrefabsPoolComponent(IPrefabsPool<GameObject> gameObjectPrefabsPool)
		{
			_gameObjectPrefabsPool = gameObjectPrefabsPool;
		}

		public int FreeElementsInPool => _gameObjectPrefabsPool.FreeElementsInPool;

		public Transform Root => _gameObjectPrefabsPool.Root;

		public int Size => _gameObjectPrefabsPool.Size;

		public IPoolReference<T> GetPoolReference(T prefab, int size = 0)
		{
			return new PrefabPoolComponentReference<T>(_gameObjectPrefabsPool.GetPoolReference(prefab.gameObject, size));
		}

		public sealed class PrefabPoolComponentReference<T> : IPoolReference<T> where T : Component
		{
			private readonly IPoolReference<GameObject> _poolReferenceToProxy;
			internal HashSet<T> PoolInstantiatedObjects => ((PrefabsPoolGameObject.PrefabPoolGameObjectReference)_poolReferenceToProxy).PoolInstantiatedObjects.Select(go => go.GetComponent<T>()).ToHashSet();

			private bool _isDisposed;

			public PrefabPoolComponentReference(IPoolReference<GameObject> poolReferenceToProxy)
			{
				_poolReferenceToProxy = poolReferenceToProxy;
			}

			public T Get(Transform parent = null)
			{
				var gameObject = _poolReferenceToProxy.Get(parent);
				return gameObject.GetComponent<T>();
			}

			public void Return(T pooledObject)
			{
				_poolReferenceToProxy.Return(pooledObject?.gameObject);
			}
			
			public void Dispose()
			{
				if(_isDisposed)
					return;
				_poolReferenceToProxy.Dispose();
				_isDisposed = true;
			}
		}
	}
}
