using UnityEngine;

namespace Package.Pooling.Infrastructures
{
	public interface IReturnPoolStrategy
	{
		void ProcessBeforeReturn(GameObject instance, Transform root);
	}
}
