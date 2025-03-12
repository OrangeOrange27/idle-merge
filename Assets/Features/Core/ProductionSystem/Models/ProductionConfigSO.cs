using UnityEngine;
using UnityEngine.Serialization;

namespace Features.Core.ProductionSystem.Models
{
    [CreateAssetMenu(fileName = "ProductionConfig", menuName = "Config/ProductionConfig")]
    public class ProductionConfigSO : ScriptableObject
    {
        [FormerlySerializedAs("ProductionConfig")] public ProductionsConfig productionsConfig = new();
    }
}