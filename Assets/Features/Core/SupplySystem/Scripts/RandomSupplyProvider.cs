using System.Linq;
using Features.Core.MergeSystem.Config;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Random = UnityEngine.Random;

namespace Features.Core.SupplySystem
{
    public class RandomSupplyProvider : ISupplyProvider
    {
        private readonly SupplyWeightsConfig _supplyWeightsConfig;
        private readonly PlaceablesFactoryResolver _placeablesFactory;

        public RandomSupplyProvider(SupplyWeightsConfig supplyWeightsConfig, PlaceablesFactoryResolver placeablesFactory)
        {
            _supplyWeightsConfig = supplyWeightsConfig;
            _placeablesFactory = placeablesFactory;
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

            var model = _placeablesFactory.Create(PlaceableType.MergeableObject);
            
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