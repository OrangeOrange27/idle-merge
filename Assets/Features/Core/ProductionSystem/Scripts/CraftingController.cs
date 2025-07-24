using System;
using System.Collections.Generic;
using System.Linq;
using Common.PlayerData;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;
using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using ZLogger;

namespace Features.Core.ProductionSystem
{
    public class CraftingController : ICraftingController
    {
        private static readonly ILogger Logger = LogManager.GetLogger<CraftingController>();

        private readonly IPlayerDataService _playerDataService;
        private readonly PlaceablesFactoryResolver _placeablesFactory;

        public CraftingController(IPlayerDataService playerDataService, PlaceablesFactoryResolver placeablesFactory)
        {
            _playerDataService = playerDataService;
            _placeablesFactory = placeablesFactory;
        }

        public void StartCrafting(ProductionBuildingModel productionBuildingModel, ProductionRecipe recipe)
        {
            if (CanStartCrafting(productionBuildingModel, recipe) == false)
            {
                Logger.ZLogWarning("Could not start crafting");
                return;
            }

            foreach (var ingredient in recipe.Ingredients)
            {
                _playerDataService.UseCollectible(ingredient.CollectibleType, ingredient.Amout);
            }

            productionBuildingModel.SelectedRecipe = recipe;
            productionBuildingModel.NextCollectionDateTime.Value =
                DateTime.Now.AddSeconds(recipe.CraftingTimeInSeconds);
            productionBuildingModel.IsCrafting.Value = true;
        }

        public List<MergeableModel> TryCollect(ProductionBuildingModel productionBuildingModel)
        {
            if (productionBuildingModel.NextCollectionDateTime.CurrentValue > DateTime.Now)
            {
                Logger.ZLogInformation(
                    $"Tried collect {productionBuildingModel.Name} but collection time has not come yet");
                return null;
            }

            return Collect(productionBuildingModel);
        }

        public bool CanStartCrafting(ProductionBuildingModel productionBuildingModel, ProductionRecipe recipe)
        {
            if (productionBuildingModel.AvailableRecipes.Contains(recipe) == false)
            {
                Logger.ZLogError(
                    $"Could not find recipe {recipe} in production building model {productionBuildingModel}");
                return false;
            }

            foreach (var component in recipe.Ingredients)
            {
                if (_playerDataService.PlayerData.Balance.GetCollectibleAmount(component.CollectibleType) <
                    component.Amout)
                {
                    Logger.ZLogInformation(
                        $"Not enough {component.CollectibleType} to start crafting {recipe.RecipeName}");
                    return false;
                }
            }

            return true;
        }
        
        private List<MergeableModel> Collect(ProductionBuildingModel productionBuildingModel)
        {
            var outcome = productionBuildingModel.SelectedRecipe.Outcome;
            var outputObjects = new List<MergeableModel>();

            foreach (var reward in outcome)
            {
                var mergeable =
                    _placeablesFactory.Create(PlaceableType.MergeableObject, reward.MergeableType) as MergeableModel;
                mergeable.Stage.Value = reward.Tier;
                outputObjects.Add(mergeable);
            }
            
            productionBuildingModel.IsCrafting.Value = false;

            return outputObjects;
        }
    }
}