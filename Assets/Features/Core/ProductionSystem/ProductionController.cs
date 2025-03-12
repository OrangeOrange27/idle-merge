using System.Collections.Generic;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
using Features.Core.ProductionSystem.Models;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core.ProductionSystem
{
    public class ProductionController : IProductionController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ProductionController>();

        private readonly Dictionary<ProductionType, ProductionConfig> _productionConfigs;

        private readonly IPlacementSystem _placementSystem;
        private readonly PlaceablesFactoryResolver _placeablesFactory;

        public ProductionController(ProductionsConfig productionsConfig, IPlacementSystem placementSystem,
            PlaceablesFactoryResolver placeablesFactory)
        {
            _placementSystem = placementSystem;
            _placeablesFactory = placeablesFactory;

            _productionConfigs = new Dictionary<ProductionType, ProductionConfig>();
            foreach (var configEntry in productionsConfig.ProductionConfigEntries)
            {
                if (_productionConfigs.ContainsKey(configEntry.ProductionType))
                    continue;

                _productionConfigs.Add(configEntry.ProductionType, configEntry.Config);
            }
        }

        public CollectibleModel TryCollect(ProductionObjectModel productionObjectModel)
        {
            if (productionObjectModel.NextCollectionDateTime.CurrentValue > System.DateTime.Now)
            {
                Logger.ZLogInformation(
                    $"Tried collect {productionObjectModel.ProductionType} but collection time has not come yet");
                return null;
            }

            if (_productionConfigs.TryGetValue(productionObjectModel.ProductionType, out var cfg) == false)
            {
                Logger.ZLogError($"No config found for {productionObjectModel.ProductionType}");
                return null;
            }

            if (productionObjectModel.TimesCollected.CurrentValue >= cfg.MaximumCollectionTimes)
            {
                Logger.ZLogWarning(
                    $"Tried collect {productionObjectModel.ProductionType} but maximum collection times reached");
                return null;
            }
            
            return Collect(productionObjectModel);
        }

        private CollectibleModel Collect(ProductionObjectModel productionObjectModel)
        {
            var collectible = CreateAndPositionCollectible(productionObjectModel);

            productionObjectModel.TimesCollected.Value++;
            productionObjectModel.NextCollectionDateTime.Value =
                System.DateTime.Now.AddSeconds(_productionConfigs[productionObjectModel.ProductionType]
                    .CoolDownInSeconds);

            return collectible;
        }

        private CollectibleModel CreateAndPositionCollectible(ProductionObjectModel productionObjectModel)
        {
            var collectible =
                _placeablesFactory.Create(PlaceableType.CollectibleObject, productionObjectModel.ProductionType);
            _placementSystem.PlaceOnRandomCell(collectible);

            return collectible as CollectibleModel;
        }
    }
}