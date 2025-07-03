using UnityEngine;

namespace Features.Core.ProgressionSystem.Models
{
    [CreateAssetMenu(fileName = "LevelsConfig", menuName = "Config/LevelsConfig")]
    public class LevelsConfigSO : ScriptableObject
    {
        public LevelsConfig LevelsConfig;
    }
}