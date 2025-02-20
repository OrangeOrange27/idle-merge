using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Strategies
{
	public abstract class DefaultPoolStrategyBase : IPoolStrategy
	{
		GameObject ICreatePoolStrategy.CreateInstance(GameObject prefab, Transform root)
		{
			var gameObject = GetPrefabInstance(prefab, root);
			if (!gameObject.activeSelf)
			{
				gameObject.SetActive(true);
			}
			
			return gameObject;
		}

		void IGetPoolStrategy.ProcessBeforeGet(GameObject instance, Transform parent)
		{
			if (instance.transform.parent != parent)
			{
				instance.transform.SetParent(parent, false);
			}
		}

		void IReturnPoolStrategy.ProcessBeforeReturn(GameObject instance, Transform root)
		{
			if (instance.transform.parent != root)
			{
				instance.transform.SetParent(root, false);
			}
		}

		protected abstract GameObject GetPrefabInstance<T>(T prefab, Transform root) where T : Object;
	}
}