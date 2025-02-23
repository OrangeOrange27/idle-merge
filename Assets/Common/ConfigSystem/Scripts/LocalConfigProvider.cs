using System;
using Common.Config.Infrastructure;
using Common.EntryPoint.Initialize;
using Common.Serialization.Infrastructure;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Config
{
    public class LocalConfigProvider<T> : IConfigProvider<T>, IBeforeAuthInitialize where T : BaseConfig
    {
        private static readonly ILogger Logger = LogManager.GetLogger<LocalConfigProvider<T>>();
        private readonly IAssetProvider _assetProvider;

        private readonly string _builtInKey;
        private readonly ISerializer _serializer;
        private T _cachedData;

        public event Action OnUpdated;

        public LocalConfigProvider(string builtInKey, IAssetProvider assetProvider, ISerializer serializer)
        {
            _assetProvider = assetProvider;
            _serializer = serializer;
            _builtInKey = builtInKey;
        }

        public async UniTask InitializeBeforeAuth()
        {
            await LoadFromBuiltIn();
        }

        public T Get()
        {
            return _cachedData;
        }

        private async UniTask LoadFromBuiltIn()
        {
            var loadAsync = await _assetProvider.LoadAsync<TextAsset>(_builtInKey);
            _cachedData = await _serializer.DeserializeAsync<T>(loadAsync.text);
            Logger.ZLogInformation("Loaded data from built-in {0}", _builtInKey);
        }
    }
}
