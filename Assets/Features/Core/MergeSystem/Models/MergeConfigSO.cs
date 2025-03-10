using UnityEngine;

namespace Features.Core.MergeSystem.Models
{
    [CreateAssetMenu(fileName = "MergeConfigSO", menuName = "Config/MergeConfig")]
    public class MergeConfigSO : ScriptableObject
    {
        public MergesConfig MergesConfig = new();
    }
}