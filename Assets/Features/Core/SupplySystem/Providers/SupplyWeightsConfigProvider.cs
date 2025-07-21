using Features.Core.SupplySystem.Models;
using Newtonsoft.Json;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.SupplySystem.Providers
{
    public class SupplyWeightsConfigProvider : MonoBehaviour, ISupplyWeightsConfigProvider
    {
        // Resources/Configs/supply_weights_config.json
        private const string ResourcePath = "Configs/supply_weights_config";
        private static readonly ILogger Logger = LogManager.GetLogger<SupplyWeightsConfigProvider>();

        private SupplyWeightsConfig _config;

        public SupplyWeightsConfig GetConfig()
        {
            if (_config == null)
            {
                LoadConfig();
            }

            return _config;
        }

        private void LoadConfig()
        {
            var jsonFile = Resources.Load<TextAsset>(ResourcePath);
            if (jsonFile == null)
            {
                Logger.ZLogError($"JSON not found in Resources at path: {ResourcePath}");
                _config = SupplyWeightsConfig.Default; // fallback to empty config
                return;
            }

            _config = JsonConvert.DeserializeObject<SupplyWeightsConfig>(jsonFile.text);
        }
    }
}