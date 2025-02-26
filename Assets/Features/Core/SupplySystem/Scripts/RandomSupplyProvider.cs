using System.Linq;
using Features.Core.MergeSystem.Config;
using Features.Core.MergeSystem.MergeableObjects;
using UnityEngine;

namespace Features.Core.SupplySystem
{
    public class RandomSupplyProvider : ISupplyProvider
    {
        private readonly SupplyWeightsConfig _supplyWeightsConfig;
        
        public RandomSupplyProvider(SupplyWeightsConfig supplyWeightsConfig)
        {
            _supplyWeightsConfig = supplyWeightsConfig;
        }
        
        public MergeableObject GetSupply()
        {
            return GetRandomSupply();
        }
        
        private MergeableObject GetRandomSupply()
        {
            var totalWeight = _supplyWeightsConfig.WeightsArray.Sum(entry => entry.Weight);
            var rnd = Random.Range(0f, totalWeight);
            var cumulativeWeight = 0f;
            
            foreach (var entry in _supplyWeightsConfig.WeightsArray)
            {
                cumulativeWeight += entry.Weight;

                if (rnd <= cumulativeWeight)
                {
                    return entry.MergeableObject;
                }
            }

            return _supplyWeightsConfig.WeightsArray[^1].MergeableObject;
        }
    }
}