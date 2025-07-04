using System;
using Common.PlayerData;
using Common.UI;
using Cysharp.Threading.Tasks;
using Features.Core.Common.Views;
using Features.Core.ProductionSystem.Components;
using Features.Core.ProductionSystem.Models;
using UnityEngine;

namespace Features.Core.ProductionSystem
{
    public interface IProductionView : IView
    {
        event Action<ProductionRecipe> OnStartProductionButtonPressedEvent;
        
        void Initialize(IPlayerDataService playerDataService,
            Func<ProductionRecipe.Reward, Transform, UniTask<ItemView>> rewardViewGetter,
            Func<string, Transform, UniTask<ItemView>> rewardItemViewGetter,
            Func<Transform, UniTask<RecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<RecipeItemView>> recipeItemViewGetter,
            Func<Transform, UniTask<IngredientItemView>> ingredientItemViewGetter);
    }
}