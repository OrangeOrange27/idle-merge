using System;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class ProductionSettings
    {
        public ProductionConfigEntry[] ProductionConfigEntries;
    }
    
    [Serializable]
    public class ProductionConfigEntry
    {
        public ProductionType ProductionType;
        public ProductionConfig Config;
    }
    
    [Serializable]
    public class ProductionConfig
    {
        public int ProductionAmount;
        public int CoolDownInSeconds;
        public int MaximumCollectionTimes;
    }
}