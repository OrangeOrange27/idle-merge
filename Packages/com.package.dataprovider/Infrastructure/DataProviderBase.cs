using Common.DataProvider.Storage.Infrastructure;
using Common.Encoding.Infrastructure;
using Common.Serialization.Infrastructure;
using Cysharp.Threading.Tasks;

namespace Common.DataProvider.Infrastructure
{
	public class DataProviderBase : IDataProvider
	{
		protected readonly IDataStorage DataStorage;
		protected readonly ISerializer Serializer;
		protected readonly IEncoder Encoder;

		public DataProviderBase(
			IDataStorage dataStorage,
			ISerializer serializer,
			IEncoder encoder
		)
		{
			DataStorage = dataStorage;
			Serializer = serializer;
			Encoder = encoder;
		}

		public virtual T Get<T>(string key)
		{
			if (string.IsNullOrEmpty(key))
				return default;

			var strValue = DataStorage.Get(key);
			strValue = Encoder.Decode(strValue);
			var data = Serializer.Deserialize<T>(strValue);
			return data;
		}

		public virtual async UniTask SetAsync<T>(string key, T value)
		{
			if (string.IsNullOrEmpty(key))
				return;
			var strValue = Serializer.Serialize(value);
			strValue = Encoder.Encode(strValue);
			await DataStorage.SetAsync(key, strValue);
		}

		public void Set<T>(string key, T value)
		{
			if (string.IsNullOrEmpty(key))
				return;

			var strValue = Serializer.Serialize(value);
			strValue = Encoder.Encode(strValue);
			DataStorage.Set(key, strValue);
		}
	}
}
