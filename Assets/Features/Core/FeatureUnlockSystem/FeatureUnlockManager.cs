using System.Collections.Generic;
using Common.Config.Infrastructure;
using Common.Utils.Extensions;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core
{
    public class FeatureUnlockManager : IFeatureUnlockManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger<FeatureUnlockManager>();
        private readonly IConfigProvider<FeaturesUnlockConfig> _configProvider;

        private FeaturesUnlockConfig Config => _configProvider.Get();

        private Dictionary<string, int> _unlockLevelDictionary;

        public FeatureUnlockManager(IConfigProvider<FeaturesUnlockConfig> configProvider)
        {
            _configProvider = configProvider;
        }

        public int GetFeatureUnlockLevel(string featureName)
        {
            if (_unlockLevelDictionary.IsNullOrEmpty())
                BuildDictionary();

            if (!_unlockLevelDictionary.TryGetValue(featureName, out var level))
            {
                Logger.ZLogWarning($"Feature unlock level for feature: {featureName} not found");
                return int.MaxValue;
            }

            return level;
        }

        private void BuildDictionary()
        {
            _unlockLevelDictionary = new Dictionary<string, int>();
            foreach (var cfg in Config.Features)
            {
                if (_unlockLevelDictionary.TryAdd(cfg.FeatureName, cfg.UnlockLevel) == false)
                {
                    Logger.ZLogWarning($"Multiple feature unlock levels for feature: {cfg.FeatureName}");
                }
            }
        }
    }
}