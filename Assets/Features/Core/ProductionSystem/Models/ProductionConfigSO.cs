using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Core.ProductionSystem.Models
{
    [CreateAssetMenu(fileName = "ProductionConfig", menuName = "Config/ProductionConfig")]
    public class ProductionConfigSO : ScriptableObject
    {
        [FormerlySerializedAs("productionsConfig")] [FormerlySerializedAs("ProductionConfig")] public ProductionSettings productionSettings = new();
    }
}