using System;
using Common.Config.Infrastructure;

namespace Features.Core
{
    [Serializable]
    public class FeatureUnlockConfigEntry
    {
        public string FeatureName;
        public int UnlockLevel;
    }

    [Serializable]
    public class FeaturesUnlockConfig : BaseConfig
    {
        public FeatureUnlockConfigEntry[] Features;
    }
}