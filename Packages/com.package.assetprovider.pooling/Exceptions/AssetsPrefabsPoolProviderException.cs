using System;

namespace Package.AssetProvider.Pooling.Exceptions
{
	public class AssetsPrefabsPoolProviderException : AssetsPrefabsPoolException
	{
		public AssetsPrefabsPoolProviderException(string message) : base(message)
		{
		}

		public AssetsPrefabsPoolProviderException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}