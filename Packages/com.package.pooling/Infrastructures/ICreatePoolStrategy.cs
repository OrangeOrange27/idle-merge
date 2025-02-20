using UnityEngine;

namespace Package.Pooling.Infrastructures
{
	public interface ICreatePoolStrategy
	{
		GameObject CreateInstance(GameObject prefab, Transform root);
	}
}
