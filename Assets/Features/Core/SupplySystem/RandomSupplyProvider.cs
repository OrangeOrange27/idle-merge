using Features.Core.MergeSystem.Config;
using UnityEngine;

namespace Features.Core.SupplySystem
{
    public class RandomSupplyProvider : ISupplyProvider
    {
        private readonly MergeableObjectsConfig _mergeableObjectsConfig;
        
        public RandomSupplyProvider(MergeableObjectsConfig mergeableObjectsConfig)
        {
            _mergeableObjectsConfig = mergeableObjectsConfig;
        }
        
        public GameAreaObject GetSupply()
        {
            return GetRandomSupply();
        }
        
        private GameAreaObject GetRandomSupply()
        {
            var rnd = Random.Range(0, _mergeableObjectsConfig.MergeableObjects.Length);
            return _mergeableObjectsConfig.MergeableObjects[rnd];
        }
    }
}