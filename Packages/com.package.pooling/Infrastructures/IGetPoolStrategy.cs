using UnityEngine;

namespace Package.Pooling.Infrastructures
{
    public interface IGetPoolStrategy
    {
        void ProcessBeforeGet(GameObject instance, Transform parent);
    }
}