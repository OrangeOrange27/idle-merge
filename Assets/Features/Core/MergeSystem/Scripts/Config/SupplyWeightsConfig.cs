using System;
using Features.Core.MergeSystem.MergeableObjects;

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
        public MergeableObject MergeableObject;
        public float Weight;
    }
}