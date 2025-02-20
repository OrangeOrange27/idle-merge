using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Strategies
{
	[AddComponentMenu("Advanced/Pool/Keep Parent Strategy")]
	[DisallowMultipleComponent]
	public sealed class KeepParentStrategy : MonoBehaviour, IGetPoolStrategy, IReturnPoolStrategy
	{
		public void ProcessBeforeGet(GameObject instance, Transform parent)
		{
			if (instance.transform.parent != parent)
			{
				instance.transform.SetParent(parent, false);
			}

			if (!instance.activeSelf)
			{
				instance.SetActive(true);
			}
		}

		public void ProcessBeforeReturn(GameObject instance, Transform root)
		{
			if (instance.activeSelf)
			{
				instance.SetActive(false);
			}
		}
	}
}
