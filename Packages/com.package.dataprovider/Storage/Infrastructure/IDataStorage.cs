using Cysharp.Threading.Tasks;

namespace Common.DataProvider.Storage.Infrastructure
{
	public interface IDataStorage
	{
		string Get(string key);
		void Set(string key, string value);
		UniTask SetAsync(string key, string value);

		void DeleteAll();
	}
}