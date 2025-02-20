using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.Pooling.Strategies
{
	[AddComponentMenu("Advanced/Pool/Disable Canvas On Creation Strategy")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Canvas))]
	public sealed class DisableCanvasOnCreationPoolStrategy : MonoBehaviour, ICreatePoolStrategy
	{
		public GameObject CreateInstance(GameObject prefab, Transform root)
		{
			var instance = Instantiate(prefab, root);

			var canvas = instance.GetComponent<Canvas>();
			canvas.enabled = false;

			return instance;
		}
	}
}
