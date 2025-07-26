using System.Collections.Generic;
using System.Linq;
using Common.Config.Infrastructure;
using Common.Data;
using Common.PlayerData;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.SupplySystem.Models;

namespace Features.Core.SupplySystem.Providers
{
    public class SupplyPoolProvider : ISupplyPoolProvider
    {
        private readonly IPlayerDataService _playerDataService;
        private readonly IFeatureUnlockManager _featureUnlockManager;
        private readonly IConfigProvider<SupplyWeightsConfig> _supplyWeightsConfigProvider;
        private readonly PlaceablesFactoryResolver _placeablesFactory;

        public SupplyPoolProvider(IPlayerDataService playerDataService, IFeatureUnlockManager featureUnlockManager,
            IConfigProvider<SupplyWeightsConfig> supplyWeightsConfigProvider)
        {
            _playerDataService = playerDataService;
            _featureUnlockManager = featureUnlockManager;
            _supplyWeightsConfigProvider = supplyWeightsConfigProvider;
        }

        public List<WeightedEntry<PlaceableModel>> GetSpawnPool()
        {
            return _supplyWeightsConfigProvider.Get().WeightsArray
                .Where(entry => IsUnlocked(entry.Item.MergeableType))
                .Select(entry => new WeightedEntry<PlaceableModel>
                {
                    Item = CreateModel(entry.Item),
                    Weight = entry.Weight
                })
                .ToList();
        }

        private bool IsUnlocked(MergeableType type)
        {
            return _featureUnlockManager.GetFeatureUnlockLevel(type.ToString()) <= _playerDataService.GetPlayerLevel();
        }

        private PlaceableModel CreateModel(MergeableObjectConfig creationDto)
        {
            var model = _placeablesFactory.Create(PlaceableType.MergeableObject, creationDto.MergeableType) as MergeableModel;
            model.Stage.Value = creationDto.Stage;
            return model;
        }
    }
}