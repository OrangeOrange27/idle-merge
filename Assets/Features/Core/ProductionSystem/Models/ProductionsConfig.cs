using System;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    //todo: rename 
    [Serializable]
    public class ProductionsConfig
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
        public int CoolDownInSeconds;
        public int MaximumCollectionTimes;
    }
}