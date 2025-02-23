using System;
using Common.Serialization.Infrastructure;
using Cysharp.Threading.Tasks;
using Package.Logger.Abstraction;
using Newtonsoft.Json;
using ZLogger;

namespace Common.Serialization.Implementation
{
	public class JsonSerializer : ISerializer
	{
		private readonly JsonSerializerSettings _jsonSerializerSettings;

		public JsonSerializer(JsonSerializerSettings jsonSerializerSettings)
		{
			_jsonSerializerSettings = jsonSerializerSettings;
		}

		public T Deserialize<T>(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return default;
			}

			if (typeof(T) == typeof(string))
			{
				return (T)Convert.ChangeType(value, typeof(T));
			}

			try
			{
				return JsonConvert.DeserializeObject<T>(value, _jsonSerializerSettings);
			}
			catch (Exception e)
			{
				LogManager.GetLogger<JsonSerializer>().ZLogError(e, "{0} | data deserialization error - type: {1}", GetType(), typeof(T).Name);
			}

			return default;
		}

		public string Serialize<T>(T value)
		{
			if (value == null)
			{
				return string.Empty;
			}

			if (value is string strValue)
			{
				return strValue;
			}
			else
			{
				return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
			}
		}

		public async UniTask<T> DeserializeAsync<T>(string value)
		{
			return await UniTask.RunOnThreadPool(() => Deserialize<T>(value));
		}

		public async UniTask<string> SerializeAsync<T>(T value)
		{
			return await UniTask.RunOnThreadPool(() => Serialize<T>(value));
		}
	}
}
