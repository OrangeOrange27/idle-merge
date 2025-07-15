using System;
using System.Collections.Generic;
using Common.PlayerData;
using Cysharp.Threading.Tasks;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public class ProductionIngredientsTabView : MonoBehaviour
    {
        [SerializeField] private Transform _contentHolder;

        private Func<Transform, UniTask<IIngredientItemView>> _ingredientItemViewGetter;
        private IPlayerDataService _playerDataService;

        private List<IIngredientItemView> _spawnedIngredientItemViews = new();

        public void Initialize(Func<Transform, UniTask<IIngredientItemView>> ingredientItemViewGetter,
            IPlayerDataService playerDataService)
        {
            _ingredientItemViewGetter = ingredientItemViewGetter;
            _playerDataService = playerDataService;

            CreateItemViews().Forget();
        }

        private async UniTask CreateItemViews()
        {
            foreach (var itemView in _spawnedIngredientItemViews)
            {
                if (itemView != null)
                    Destroy(itemView.GameObject);
            }

            _spawnedIngredientItemViews.Clear();

            foreach (CollectibleType type in Enum.GetValues(typeof(CollectibleType)))
            {
                var itemView = await _ingredientItemViewGetter.Invoke(_contentHolder);
                var amount = _playerDataService.PlayerBalance.GetCollectibleAmount(type);

                itemView.SetText(amount.ToString());
                itemView.SetType(type);

                _spawnedIngredientItemViews.Add(itemView);
            }
        }
    }
}