using UnityEngine;

namespace Features.Core.ProductionSystem.Models
{
    [CreateAssetMenu(fileName = "RecyclingConfig", menuName = "Config/RecyclingConfig")]
    public class RecyclingConfigSO : ScriptableObject
    {
        public RecyclingConfig RecyclingConfig = new();
    }
}