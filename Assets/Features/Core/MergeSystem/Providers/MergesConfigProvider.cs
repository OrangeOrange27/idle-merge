using Features.Core.MergeSystem.Models;
using UnityEngine;

namespace Features.Core.MergeSystem.Providers
{
    public class MergesConfigProvider : MonoBehaviour, IMergesConfigProvider
    {
        [SerializeField] private MergeConfigSO _config;

        public MergesConfig GetConfig() => _config.MergesConfig;
    }
}