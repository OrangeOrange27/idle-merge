using Features.Core.MergeSystem.Config;
using UnityEngine;

namespace Features.Core.MergeSystem.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MergeableObjects", menuName = "Config/MergeableObjects")]
    public class MergeableObjectsConfigSO : ScriptableObject
    {
        public MergeableObjectsConfig MergeableObjectsConfig = new();
    }
}