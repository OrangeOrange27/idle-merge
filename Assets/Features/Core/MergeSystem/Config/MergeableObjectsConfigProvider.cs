using Features.Core.MergeSystem.ScriptableObjects;
using UnityEngine;

namespace Features.Core.MergeSystem.Config
{
    public class MergeableObjectsConfigProvider : MonoBehaviour, IMergeableObjectsConfigProvider
    {
        [SerializeField] private MergeableObjectsConfigSO _config;

        public MergeableObjectsConfig GetConfig() => _config.MergeableObjectsConfig;
    }
}