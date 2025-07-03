using Features.Core.ProgressionSystem.Models;
using UnityEngine;

namespace Features.Core.ProgressionSystem.Providers
{
    public class LevelsConfigProvider : MonoBehaviour, ILevelsConfigProvider
    {
        [SerializeField] private LevelsConfigSO _config;

        public LevelsConfig GetConfig() => _config.LevelsConfig;
    }
}