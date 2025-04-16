using System;
using System.Collections.Generic;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core.Placeables.Factories
{
    public class ProductionObjectsFactory : IPlaceablesFactory<ProductionType>
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ProductionObjectsFactory>();
        private readonly Dictionary<ProductionType, ProductionConfig> _productionConfigs;
        private readonly Dictionary<ProductionType, RecycleResult> _recycleConfigs;
        
        public PlaceableType FactoryType => PlaceableType.ProductionEntity;
        
        public ProductionObjectsFactory(ProductionSettings productionSettings, RecyclingConfig recyclingConfig)
        {
            _productionConfigs = new Dictionary<ProductionType, ProductionConfig>();
            foreach (var configEntry in productionSettings.ProductionConfigEntries)
            {
                if (_productionConfigs.ContainsKey(configEntry.ProductionType))
                    continue;

                _productionConfigs.Add(configEntry.ProductionType, configEntry.Config);
            }
            
            _recycleConfigs = new Dictionary<ProductionType, RecycleResult>();
            foreach (var configEntry in recyclingConfig.RecyclingConfigEntries)
            {
                if (_recycleConfigs.ContainsKey(configEntry.ProductionType))
                    continue;

                _recycleConfigs.Add(configEntry.ProductionType, configEntry.Config);
            }
        }

        public PlaceableModel Create(ProductionType type)
        {
            if (_productionConfigs.TryGetValue(type, out var productionConfig) == false)
            {
                Logger.ZLogError($"No production config found for {type}");
                return null;
            }
            
            if (_recycleConfigs.TryGetValue(type, out var recycleResult) == false)
            {
                Logger.ZLogError($"No recycle config found for {type}");
                return null;
            }
            
            var model = new ProductionObjectModel() { ObjectType = PlaceableType.ProductionEntity, ProductionType = type };
            model.ProductionConfig = productionConfig;
            model.RecycleResult = recycleResult;
            return model;
        }

        public PlaceableModel Create(Enum objectType)
        {
            return objectType is ProductionType type
                ? Create(type)
                : throw new ArgumentException($"Invalid type {objectType}");
        }
    }
}