using Features.Core.SupplySystem.Config;
using Features.Core.SupplySystem.ScriptableObjects;
using UnityEngine;

namespace Features.Core.SupplySystem.Providers
{
    public class SupplyWeightsConfigProvider : MonoBehaviour, ISupplyWeightsConfigProvider
    {
        [SerializeField] private SupplyWeightsConfigSO _config;

        public SupplyWeightsConfig GetConfig() => _config.SupplyWeightsConfig;
    }
}