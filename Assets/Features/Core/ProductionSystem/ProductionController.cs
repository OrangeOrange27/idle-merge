using System;
using System.Collections.Generic;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.PlacementSystem;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core.ProductionSystem
{
    public class ProductionController : IProductionController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ProductionController>();

        private readonly IPlacementSystem _placementSystem;
        private readonly PlaceablesFactoryResolver _placeablesFactory;

        public ProductionController(IPlacementSystem placementSystem, PlaceablesFactoryResolver placeablesFactory)
        {
            _placementSystem = placementSystem;
            _placeablesFactory = placeablesFactory;
        }

        public List<CollectibleModel> TryCollect(ProductionObjectModel productionObjectModel)
        {
            if (productionObjectModel.NextCollectionDateTime.CurrentValue > System.DateTime.Now)
            {
                Logger.ZLogInformation(
                    $"Tried collect {productionObjectModel.ProductionType} but collection time has not come yet");
                return null;
            }

            if (productionObjectModel.TimesCollected.CurrentValue >=
                productionObjectModel.ProductionConfig.MaximumCollectionTimes)
            {
                Logger.ZLogWarning(
                    $"Tried collect {productionObjectModel.ProductionType} but maximum collection times reached");
                return null;
            }

            return Collect(productionObjectModel);
        }

        public List<PlaceableModel> Recycle(ProductionObjectModel productionObjectModel)
        {
            var instruction = productionObjectModel.RecycleResult.RecycleResultPlaceable;
            Enum type = instruction.ObjectType switch
            {
                PlaceableType.CollectibleObject => instruction.CollectibleType,
                PlaceableType.MergeableObject => instruction.MergeableType,
                PlaceableType.ProductionEntity => instruction.ProductionType,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            var placeables = new List<PlaceableModel>();
            for (var i = 0; i < productionObjectModel.RecycleResult.Amount; i++)
            {
                var placeable = _placeablesFactory.Create(instruction.ObjectType, type);
                _placementSystem.PlaceOnRandomCell(placeable);
                placeables.Add(placeable);
            }

            return placeables;
        }

        private List<CollectibleModel> Collect(ProductionObjectModel productionObjectModel)
        {
            var collectibles = new List<CollectibleModel>();
            for (var i = 0; i < productionObjectModel.ProductionConfig.ProductionAmount; i++)
            {
                collectibles.Add(CreateAndPositionCollectible(productionObjectModel));
            }
            
            productionObjectModel.NextCollectionDateTime.Value =
                DateTime.Now.AddSeconds(productionObjectModel.ProductionConfig.CoolDownInSeconds);
            productionObjectModel.TimesCollected.Value++;

            return collectibles;
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