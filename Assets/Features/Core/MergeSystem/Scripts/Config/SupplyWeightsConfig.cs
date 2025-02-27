using System;
using Features.Core.MergeSystem.Scripts.Models;

namespace Features.Core.MergeSystem.Config
{
    [Serializable]
    public class SupplyWeightsConfig
    {
        public SupplyWeightsConfigEntry[] WeightsArray;
    }
    
    [Serializable]
    public class SupplyWeightsConfigEntry
    {
        public MergeableType MergeableObject;
        public float Weight;
    }
}