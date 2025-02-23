namespace Common.Encoding.Infrastructure
{
	/// <summary>
	/// when data arrives from/to device, it may be in different formats
	/// on different platforms, so we may need to convert data
	///
	/// meant to be used in a data providers
	/// </summary>
	public interface IEncoder
	{
		string Encode(string sourceData);

		string Decode(string sourceData);
	}
}