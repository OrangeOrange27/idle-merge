using System;
using Common.Encoding.Infrastructure;
using Microsoft.Extensions.Logging;
using ZLogger;

namespace Common.Encoding.Implementation
{
	public class CryptEncoder : IEncoder
	{
		private const string PassPhrase = "wfjkgb^34rjvhg(";

		private static readonly ILogger Logger = Package.Logger.Abstraction.LogManager.GetLogger<CryptEncoder>();
		public string Encode(string sourceData)
		{
			if (string.IsNullOrEmpty(sourceData))
				return sourceData;

			return StringCipher.Encrypt(sourceData, PassPhrase);
		}

		public string Decode(string sourceData)
		{
			if (string.IsNullOrEmpty(sourceData))
				return sourceData;

			var decoded = sourceData;

			try
			{
				decoded = StringCipher.Decrypt(sourceData, PassPhrase);
			}
			catch (Exception e)
			{
				Logger.ZLogError(e, "Failed to decrypt. Return not decrypted");
			}

			return decoded;
		}
	}
}
