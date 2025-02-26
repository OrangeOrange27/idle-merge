using Features.Core.MergeSystem.Config;
using UnityEngine;

namespace Features.Core.MergeSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SupplyWeightsConfig", menuName = "Config/SupplyWeightsConfig")]
    public class SupplyWeightsConfigSO : ScriptableObject
    {
        public SupplyWeightsConfig SupplyWeightsConfig = new();
    }
}