using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Models;
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
        private Func<Transform, UniTask<IRecipeComponentView>> _recipeComponentViewGetter;
        private Func<string, Transform, UniTask<IItemView>> _rewardItemViewGetter;

        private List<IRecipeComponentView> _spawnedViews = new();
        private IItemView _rewardItemView;

        public void Initialize(IPlayerDataService playerDataService,
            Func<Transform, UniTask<IRecipeComponentView>> recipeComponentViewGetter,
            Func<string, Transform, UniTask<IItemView>> rewardItemViewGetter)
        {
            _playerDataService = playerDataService;
            _recipeComponentViewGetter = recipeComponentViewGetter;
            _rewardItemViewGetter = rewardItemViewGetter;
        }

        public async UniTask SetRecipe(ProductionRecipe recipe, CancellationToken token)
        {
            Clear();

            foreach (var recipeComponent in recipe.Ingredients)
            {
                token.ThrowIfCancellationRequested();

                var itemType = recipeComponent.CollectibleType;
                var itemView = await _recipeComponentViewGetter.Invoke(GetIngredientContainer());

                token.ThrowIfCancellationRequested();

                itemView.SetText(
                    $"{_playerDataService.PlayerBalance.GetCollectibleAmount(itemType)} / {recipeComponent.Amout}");

                _spawnedViews.Add(itemView);

                if (_spawnedViews.Count > 1 && _spawnedViews.Count < _plusSigns.Length)
                    _plusSigns[_spawnedViews.Count - 2].gameObject.SetActive(true);
            }

            token.ThrowIfCancellationRequested();

            _rewardItemView = await _rewardItemViewGetter.Invoke(recipe.RecipeName, _rewardItemContainer);

            token.ThrowIfCancellationRequested();
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
                    Destroy(view.GameObject);
            }
            _spawnedViews.Clear();

            if (_rewardItemView != null)
            {
                Destroy(_rewardItemView.GameObject);
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