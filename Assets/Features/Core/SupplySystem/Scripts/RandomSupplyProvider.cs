using System.Linq;
using Features.Core.MergeSystem.Config;
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
        
        public PlaceableModel GetSupply()
        {
            return GetRandomSupply();
        }
        
        private PlaceableModel GetRandomSupply()
        {
            var totalWeight = _supplyWeightsConfig.WeightsArray.Sum(entry => entry.Weight);
            var rnd = Random.Range(0f, totalWeight);
            var cumulativeWeight = 0f;

            var model = new PlaceableModel()
            {
                ObjectType = GameAreaObjectType.MergeableObject
            };
            
            foreach (var entry in _supplyWeightsConfig.WeightsArray)
            {
                cumulativeWeight += entry.Weight;

                if (rnd <= cumulativeWeight)
                {
                    model.MergeableType = entry.MergeableObject.MergeableType;
                    model.Stage.Value = entry.MergeableObject.Stage;
                    return model;
                }
            }

            var objectConfig = _supplyWeightsConfig.WeightsArray[^1].MergeableObject;
            model.MergeableType = objectConfig.MergeableType;
            model.Stage.Value = objectConfig.Stage;
            return model;
        }
    }
}