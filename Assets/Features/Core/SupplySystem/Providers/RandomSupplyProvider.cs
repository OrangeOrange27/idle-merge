using System.Linq;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.SupplySystem.Config;
using Features.Core.SupplySystem.Models;
using Random = UnityEngine.Random;

namespace Features.Core.SupplySystem.Providers
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

            var dto = new MergeableCreationDTO();
            
            foreach (var entry in _supplyWeightsConfig.WeightsArray)
            {
                cumulativeWeight += entry.Weight;

                if (rnd <= cumulativeWeight)
                {
                    dto.Type = entry.MergeableObject.MergeableType;
                    dto.Stage = entry.MergeableObject.Stage;
                    return CreateModel(dto);
                }
            }

            var objectConfig = _supplyWeightsConfig.WeightsArray[^1].MergeableObject;
            dto.Type = objectConfig.MergeableType;
            dto.Stage = objectConfig.Stage;
            return CreateModel(dto);
            
            PlaceableModel CreateModel(MergeableCreationDTO creationDto)
            {
                var model = _placeablesFactory.Create(PlaceableType.MergeableObject, creationDto.Type);
                model.Stage.Value = creationDto.Stage;
                return model;
            }
        }
    }
}