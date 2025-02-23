using Cysharp.Threading.Tasks;

namespace Common.DataProvider.Infrastructure
{
	public interface IDataProvider
	{
		T Get<T>(string key);
		UniTask SetAsync<T>(string key, T value);
		void Set<T>(string key, T value);
	}
}