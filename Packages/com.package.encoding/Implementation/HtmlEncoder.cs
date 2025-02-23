using System.Net;
using System.Text.RegularExpressions;
using Common.Encoding.Infrastructure;

namespace Common.Encoding.Implementation
{
	public class HtmlEncoder : IEncoder
	{
		public string Encode(string sourceData)
		{
			return string.IsNullOrEmpty(sourceData) ? sourceData : WebUtility.HtmlEncode(sourceData);
		}

		public string Decode(string sourceData)
		{
			return IsDecodable(sourceData) ? WebUtility.HtmlDecode(sourceData) : sourceData;
		}

		private static bool IsDecodable(string sourceData)
		{
			if (string.IsNullOrEmpty(sourceData))
			{
				return false;
			}

			sourceData = sourceData.Trim();
			return Regex.IsMatch(sourceData, @"^{[\n\r\t]*(&#10;|&#9;|&quot;)", RegexOptions.None);
		}
	}
}