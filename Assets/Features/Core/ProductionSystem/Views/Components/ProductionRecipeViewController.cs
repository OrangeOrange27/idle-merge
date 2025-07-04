using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using Features.Core.ProductionSystem.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.ProductionSystem.Components
{
    public class ProductionRecipeViewController : MonoBehaviour
    {
        private static readonly ILogger Logger = LogManager.GetLogger<ProductionRecipeViewController>();

        [SerializeField] private Transform _rewardItemContainer;
        [SerializeField] private Transform[] _itemContainers;
        [SerializeField] private Transform[] _plusSigns;

        private IPlayerDataService _playerDataService;
        private IViewLoader<RecipeComponentView> _itemsViewLoader;
        private IViewLoader<ItemView, string> _rewardItemViewLoader;

        private List<RecipeComponentView> _spawnedViews = new();
        private ItemView _rewardItemView;

        public void Initialize(IPlayerDataService playerDataService,
            IViewLoader<RecipeComponentView> itemsViewLoader,
            IViewLoader<ItemView, string> rewardItemViewLoader)
        {
            _playerDataService = playerDataService;
            _itemsViewLoader = itemsViewLoader;
            _rewardItemViewLoader = rewardItemViewLoader;
        }

        public async UniTask SetRecipe(ProductionRecipe recipe, ICollection<IDisposable> resources,
            CancellationToken token)
        {
            Clear();
            
            foreach (var recipeComponent in recipe.InComponents)
            {
                var itemType = recipeComponent.CollectibleType;
                var itemView = await _itemsViewLoader.Load(resources, token,
                    GetIngredientContainer());

                itemView.SetText(
                    $"{_playerDataService.PlayerBalance.GetCollectibleAmount(itemType)} / {recipeComponent.Amout}");

                _spawnedViews.Add(itemView);

                if (_spawnedViews.Count > 1 && _spawnedViews.Count < _plusSigns.Length)
                    _plusSigns[_spawnedViews.Count - 2].gameObject.SetActive(true);
            }

            _rewardItemView =
                await _rewardItemViewLoader.Load(recipe.RecipeName, resources, token, _rewardItemContainer);
        }

        private Transform GetIngredientContainer()
        {
            if (_spawnedViews.Count >= _itemContainers.Length)
            {
                Logger.ZLogError($"Could not find recipe container for {_itemContainers.Length} items");
                return null;
            }

            return _itemContainers[_spawnedViews.Count];
        }

        private void Clear()
        {
            foreach (var view in _spawnedViews)
            {
                if(view!=null) 
                    Destroy(view.gameObject);
            }
            _spawnedViews.Clear();

            if (_rewardItemView != null)
            {
                Destroy(_rewardItemView.gameObject);
                _rewardItemView = null;
            }

            foreach (var plusSign in _plusSigns)
            {
                plusSign.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            Clear();
        }
    }
}