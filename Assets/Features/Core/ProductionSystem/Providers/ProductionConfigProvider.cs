using Features.Core.MergeSystem.Models;
using Features.Core.ProductionSystem.Models;
using UnityEngine;

namespace Features.Core.ProductionSystem.Providers
{
    public class ProductionConfigProvider : MonoBehaviour, IProductionConfigProvider
    {
        [SerializeField] private ProductionConfigSO _config;

        public ProductionsConfig GetConfig() => _config.productionsConfig;
    }
}