using System;
using Features.Core.MergeSystem.Models;

namespace Features.Core.SupplySystem.Config
{
    [Serializable]
    public class SupplyWeightsConfig
    {
        public SupplyWeightsConfigEntry[] WeightsArray;
    }
    
    [Serializable]
    public class SupplyWeightsConfigEntry
    {
        public MergeableObjectConfig MergeableObject;
        public float Weight;
    }
    
    [Serializable]
    public class MergeableObjectConfig
    {
        public MergeableType MergeableType;
        public int Stage;
    }
}