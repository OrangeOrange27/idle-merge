using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Strategies
{
	[AddComponentMenu("Advanced/Pool/Disable On Start Strategy")]
	[DisallowMultipleComponent]
	public class DisableOnStartStrategy : MonoBehaviour, IPoolStrategy
	{
		public GameObject CreateInstance(GameObject prefab, Transform root)
		{
			var instance = Instantiate(prefab, root);

			return instance;
		}

		public void ProcessBeforeGet(GameObject instance, Transform parent)
		{
			if (instance.transform.parent != parent)
			{
				instance.transform.SetParent(parent, false);
			}

			if (instance.activeSelf)
			{
				instance.SetActive(false);
			}
		}

		public void ProcessBeforeReturn(GameObject instance, Transform root)
		{
			if (instance.activeSelf)
			{
				instance.SetActive(false);
			}

			if (instance.transform.parent != root)
			{
				instance.transform.SetParent(root, false);
			}
		}
	}
}