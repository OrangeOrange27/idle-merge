using System;

namespace Package.AssetProvider.Pooling.Exceptions
{
	public class AssetsPrefabsPoolException : Exception
	{
		public AssetsPrefabsPoolException(string message) : base(message)
		{
		}

		public AssetsPrefabsPoolException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}