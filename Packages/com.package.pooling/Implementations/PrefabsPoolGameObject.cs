using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Package.Pooling.Infrastructures;
using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using Object = UnityEngine.Object;

namespace Package.Pooling.Implementations
{
	public sealed partial class PrefabsPoolGameObject : IPrefabsPool<GameObject> 
	{ 
		public const int MaxSize = 300_000;

		private readonly ICreatePoolStrategy _defaultCreatePoolStrategy;
		private readonly IGetPoolStrategy _defaultGetPoolStrategy;
		private readonly IReturnPoolStrategy _defaultReturnPoolStrategy;
		private readonly ILogger _logger;

		private readonly Queue<GameObject> _queue = new Queue<GameObject>();

		private ICreatePoolStrategy _createPoolStrategy;
		private IGetPoolStrategy _getPoolStrategy;
		private bool _isInitialized;

		private GameObject _prefab;
		private int _referenceCount;
		private IReturnPoolStrategy _returnPoolStrategy;

		public PrefabsPoolGameObject(
			ICreatePoolStrategy createPoolStrategy,
			IGetPoolStrategy getPoolStrategy,
			IReturnPoolStrategy returnPoolStrategy, Func<Type, ILogger> loggerFactory)
		{
			_createPoolStrategy = _defaultCreatePoolStrategy = createPoolStrategy;
			_getPoolStrategy = _defaultGetPoolStrategy = getPoolStrategy;
			_returnPoolStrategy = _defaultReturnPoolStrategy = returnPoolStrategy;
			_logger = loggerFactory.Invoke(GetType());
		}

		public Transform Root { get; private set; }
		public int Size { get; private set; }

		public int FreeElementsInPool => _queue.Count;

		public IPoolReference<GameObject> GetPoolReference(GameObject prefab, int size = 0)
		{
			++_referenceCount;

			if (!_isInitialized) Initialize(prefab);

			Resize(size);

			return new PrefabPoolGameObjectReference(this, _logger);
		}

		private void Initialize(GameObject prefab)
		{
			_isInitialized = true;
			_prefab = prefab;

			var rootGameObject = new GameObject("Pool_GameObject");
			rootGameObject.SetActive(false);
			Object.DontDestroyOnLoad(rootGameObject);
			Root = rootGameObject.transform;

			if (prefab.TryGetComponent(out ICreatePoolStrategy createPoolStrategy))
				_createPoolStrategy = createPoolStrategy;

			if (prefab.TryGetComponent(out IGetPoolStrategy getPoolStrategy)) _getPoolStrategy = getPoolStrategy;

			if (prefab.TryGetComponent(out IReturnPoolStrategy returnPoolStrategy))
				_returnPoolStrategy = returnPoolStrategy;
		}

		private void Resize(int size)
		{
			while (Size < size)
			{
				var instance = _createPoolStrategy.CreateInstance(_prefab, Root);
				_queue.Enqueue(instance);
				++Size;
			}
		}

		private GameObject Get(Transform parent)
		{
			GameObject instance;
			if (_queue.Count > 0)
			{
				instance = _queue.Dequeue();
				if (instance == null)
				{
					_logger.LogWarning("The instance in the pool was destroyed outside");
					instance = _createPoolStrategy.CreateInstance(_prefab, parent);
				}
			}
			else
			{
				instance = _createPoolStrategy.CreateInstance(_prefab, parent);
				++Size;
			}

			foreach (var prefabPoolCallbackse in instance.GetComponents<IPrefabPoolCallbacks>())
			{
				prefabPoolCallbackse.BeforeSpawn();
			}			
			_getPoolStrategy.ProcessBeforeGet(instance.gameObject, parent);
			return instance;
		}

		private void Return(GameObject instance)
		{
			if (instance == null)
			{
				_logger.LogWarning("A null or destroyed object cannot be returned to the pool.");
				return;
			}

			if (_queue.Contains(instance))
			{
				_logger.LogWarning(
					$"Object {instance.name} with instance id {instance.GetInstanceID()} already exist in pool.");
				return;
			}

			if (_queue.Count >= MaxSize)
			{
				_logger.LogWarning(
					$"The pool cannot contain more than {MaxSize} items. The returned object will be destroyed.");

				Object.Destroy(instance);
				return;
			}

			foreach (var prefabPoolCallbackse in instance.GetComponents<IPrefabPoolCallbacks>())
			{
				prefabPoolCallbackse.AfterReturnToPool();
			}
			_returnPoolStrategy.ProcessBeforeReturn(instance.gameObject, Root);
			_queue.Enqueue(instance);
		}

		private void PoolReferenceDisposed()
		{
			--_referenceCount;

			if (_referenceCount <= 0) Clear();
		}

		private void Clear()
		{
			foreach (var component in _queue) Object.Destroy(component.gameObject);

			Object.Destroy(Root.gameObject);

			_createPoolStrategy = _defaultCreatePoolStrategy;
			_getPoolStrategy = _defaultGetPoolStrategy;
			_returnPoolStrategy = _defaultReturnPoolStrategy;

			_queue.Clear();
			Root = null;
			_prefab = null;
			_isInitialized = false;
			_referenceCount = 0;
			Size = 0;
		}
	}
}
