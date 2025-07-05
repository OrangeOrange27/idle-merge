using Features.Core.ProductionSystem.Models;
using UnityEngine;

namespace Features.Core.ProductionSystem.Providers
{
    public class RecycleConfigProvider : MonoBehaviour, IRecycleConfigProvider
    {
        [SerializeField] private RecyclingConfigSO _config;

        public RecyclingConfig GetConfig() => _config.RecyclingConfig;
    }
}