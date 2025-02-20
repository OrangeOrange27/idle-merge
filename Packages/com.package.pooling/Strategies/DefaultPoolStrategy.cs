using Microsoft.Extensions.Logging;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Package.Pooling.Strategies
{
	public sealed class DefaultPoolStrategy : DefaultPoolStrategyBase
	{
		private static readonly ILogger Logger = Package.Logger.Abstraction.LogManager.GetLogger<DefaultPoolStrategy>();
		protected override GameObject GetPrefabInstance<T>(T prefab, Transform root)
		{
			var instantiate = Object.Instantiate(prefab, root);
			if (instantiate is GameObject go)
			{
				return go;
			}
			else if (instantiate is Component component)
			{
				return component.gameObject;
			}

			Logger.LogError($"Trying to instatiate {typeof(T)} but supports only GameObject and Components");
			return null;
		}
	}
}