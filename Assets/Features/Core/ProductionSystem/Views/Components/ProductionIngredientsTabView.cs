using System;
using System.Collections.Generic;
using System.Threading;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Placeables.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public class ProductionIngredientsTabView : MonoBehaviour
    {
        [SerializeField] private Transform _contentHolder;
        
        private IViewLoader<IngredientItemView> _itemViewLoader;
        private IPlayerDataService _playerDataService;
        private IControllerResources _controllerResources;
        private CancellationToken _cancellationToken;
        
        private List<IngredientItemView> _spawnedIngredientItemViews = new();

        public void Initialize(IViewLoader<IngredientItemView> itemViewLoader, IPlayerDataService playerDataService,
            IControllerResources controllerResources, CancellationToken token)
        {
            _itemViewLoader = itemViewLoader;
            _playerDataService = playerDataService;
            _controllerResources = controllerResources;
            _cancellationToken = token;

            CreateItemViews().Forget();
        }

        private async UniTask CreateItemViews()
        {
            foreach (var itemView in _spawnedIngredientItemViews)
            {
                if(itemView!=null)
                    Destroy(itemView.gameObject);
            }
            _spawnedIngredientItemViews.Clear();
            
            foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
            {
                var itemView = await _itemViewLoader.Load(_controllerResources, _cancellationToken, _contentHolder);
                var amount = _playerDataService.PlayerBalance.GetCollectibleAmount(type);
                
                itemView.SetText(amount.ToString());
                itemView.SetIngredientType(type);
                
                _spawnedIngredientItemViews.Add(itemView);
            }
        }
    }
}