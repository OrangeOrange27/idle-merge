using Common.Encoding.Infrastructure;

namespace Common.Encoding.Implementation
{
	public class GenericEncoder : IEncoder
	{
		public string Encode(string sourceData)
		{
			return sourceData;
		}

		public string Decode(string sourceData)
		{
			return sourceData;
		}
	}
}