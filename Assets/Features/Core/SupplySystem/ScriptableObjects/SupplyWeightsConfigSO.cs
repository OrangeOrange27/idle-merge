using Features.Core.SupplySystem.Config;
using UnityEngine;

namespace Features.Core.SupplySystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "SupplyWeightsConfig", menuName = "Config/SupplyWeightsConfig")]
    public class SupplyWeightsConfigSO : ScriptableObject
    {
        public SupplyWeightsConfig SupplyWeightsConfig = new();
    }
}