using Cysharp.Threading.Tasks;

namespace Common.Serialization.Infrastructure
{
	public interface ISerializer
	{
		T Deserialize<T>(string value);
		string Serialize<T>(T value);

		UniTask<T> DeserializeAsync<T>(string value);
		UniTask<string> SerializeAsync<T>(T value);
	}
}