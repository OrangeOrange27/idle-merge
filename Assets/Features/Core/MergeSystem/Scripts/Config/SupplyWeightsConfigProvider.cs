using Features.Core.MergeSystem.ScriptableObjects;
using UnityEngine;

namespace Features.Core.MergeSystem.Config
{
    public class SupplyWeightsConfigProvider : MonoBehaviour, ISupplyWeightsConfigProvider
    {
        [SerializeField] private SupplyWeightsConfigSO _config;

        public SupplyWeightsConfig GetConfig() => _config.SupplyWeightsConfig;
    }
}