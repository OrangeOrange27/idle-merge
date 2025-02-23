using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Common.Config.Infrastructure;
using Common.DataProvider.Infrastructure;
using Common.EntryPoint.Initialize;
using Common.Serialization.Infrastructure;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using UnityEngine.Networking;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Common.Config
{
    public class RemoteConfigProvider<T> : IConfigProvider<T>, IBeforeAuthInitialize where T : BaseConfig
    {
        private static readonly ILogger Logger = LogManager.GetLogger<RemoteConfigProvider<T>>();
        private readonly IAssetProvider _assetProvider;

        private readonly string _builtInKey;
        private readonly IDataProvider _dataProvider;
        private readonly Uri _remoteUrl;
        private readonly ISerializer _serializer;
        private T _cachedData;

        public RemoteConfigProvider(string builtInKey, Uri remoteUrl, IDataProvider dataProvider, IAssetProvider assetProvider, ISerializer serializer)
        {
            _dataProvider = dataProvider;
            _assetProvider = assetProvider;
            _serializer = serializer;
            _builtInKey = builtInKey;
            _remoteUrl = remoteUrl;
        }

        public async UniTask InitializeBeforeAuth()
        {
            await LoadFromCacheOrBuiltIn();
            await TryUpdateFromServer();
        }

        public event Action OnUpdated;

        public T Get()
        {
            return _cachedData;
        }

        private async UniTask LoadFromCacheOrBuiltIn()
        {
            Logger.ZLogInformation("Try load from cache");
            _cachedData = _dataProvider.Get<T>(_builtInKey);
            if (_cachedData == null)
            {
                Logger.ZLogInformation("No data in cache, load from built in");

                var loadAsync = await _assetProvider.LoadAsync<TextAsset>(_builtInKey);
                _cachedData = await _serializer.DeserializeAsync<T>(loadAsync.text);
                Logger.ZLogInformation("Loaded data from built-in {0}", _builtInKey);
            }
            else
            {
                Logger.ZLogInformation("Loaded data from cache. Hashcode: {0}", _cachedData.Hash);
            }
        }

        private async UniTask<T> LoadConfigFromRemote()
        {
            try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, _remoteUrl);
                
                // Set request headers
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                request.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

                using var response = await client.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    Logger.ZLogError($"Failed to download config: {response.StatusCode} {response.ReasonPhrase}");
                    return default;
                }

                var contentLength = response.Content.Headers.ContentLength;
                Logger.ZLogInformation($"Downloaded bytes: {contentLength}");

                var content = await response.Content.ReadAsByteArrayAsync();
                string decompressedJson;

                if (IsGzipped(response))
                {
                    decompressedJson = await DecompressGzipAsync(content);
                    Logger.ZLogInformation($"Decompressed content length: {decompressedJson.Length}");
                }
                else
                {
                    decompressedJson = System.Text.Encoding.UTF8.GetString(content);
                    Logger.ZLogInformation("Content was not gzipped");
                }

                var deserializedObject = await _serializer.DeserializeAsync<T>(decompressedJson);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                Logger.ZLogError(ex, "Error loading remote config");
                return default;
            }
        }

        private bool IsGzipped(HttpResponseMessage response)
        {
            // Check Content-Encoding in response content headers
            return response.Content.Headers.ContentEncoding.Any(encoding => 
                encoding.Equals("gzip", StringComparison.OrdinalIgnoreCase));
        }

        private async UniTask<string> DecompressGzipAsync(byte[] compressedData)
        {
            try
            {
                using var compressedStream = new MemoryStream(compressedData);
                using var gzipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
                using var decompressedStream = new MemoryStream();
                
                await gzipStream.CopyToAsync(decompressedStream);
                decompressedStream.Position = 0;
                
                using var reader = new StreamReader(decompressedStream);
                return await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                Logger.ZLogError(ex, "Error decompressing gzipped content");
                throw;
            }
        }

        private async UniTask TryUpdateFromServer()
        {
            string remoteConfigHash = null;
            try
            {
                Logger.ZLogInformation("Start TryUpdateFromServer by url: {0}", _remoteUrl);
                remoteConfigHash = await GetRemoteFileHashAsync();
            }
            catch (Exception e)
            {
                Logger.ZLogError(e, "Can't get remote hash");
                return;
            }

            if (_cachedData.Hash != remoteConfigHash)
            {
                Logger.ZLogInformation("Remote hashcode: {1} Hashcode local: {0}", _cachedData.Hash, remoteConfigHash);

                var loadConfigFromRemote = await LoadConfigFromRemote();
                if (loadConfigFromRemote != default)
                {
                    _cachedData = loadConfigFromRemote;
                    _cachedData.Hash = remoteConfigHash;
                    Logger.ZLogInformation("Config was updated from remote. hash: {0}", _cachedData.Hash);

                    OnUpdated?.Invoke();
                    _dataProvider.SetAsync(_builtInKey, _cachedData).Forget();
                }
            }
        }

        private async UniTask<string> GetRemoteFileHashAsync()
        {
            using var request = UnityWebRequest.Head(_remoteUrl);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Logger.ZLogError("Error fetching remote file headers: {0}", request.error);
                return null;
            }

            return GetHashBasedOnETag(request);
        }

        private string GetHashBasedOnETag(UnityWebRequest request)
        {
            var etag = request.GetResponseHeader("ETag");
            return etag?.Trim('"');
        }
    }
}