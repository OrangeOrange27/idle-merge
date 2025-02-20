using System;
using System.Text;

namespace Package.AssetProvider.Implementation
{
	public class AssetProviderException : Exception
	{
		public readonly string Key;

		private readonly string _message;

		public AssetProviderException(string key, Exception innerException)
		{
			Key = key;
			_message = string.Format("Error while downloading assets: {0} --> {1}", key, innerException.ToString());
		}

		public AssetProviderException(string[] keys, Exception innerException) : this(GetKeyFromKeys(keys),
			innerException)
		{
		}

		public override string Message => _message;

		public override string ToString()
		{
			return $"AssetProviderException: AssetKey: {Key}";
		}

		private static string GetKeyFromKeys(string[] keys)
		{
			var sb = new StringBuilder(keys[0]);

			for (var i = 1; i < keys.Length; i++) sb.AppendFormat(", {0}", keys[i]);

			return sb.ToString();
		}
	}
}
