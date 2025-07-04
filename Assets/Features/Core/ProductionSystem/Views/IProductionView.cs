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
            Func<string, Transform, UniTask<IItemView>> rewardItemViewGetter,
            Func<Transform, UniTask<IRecipeComponentView>> recipeComponentViewGetter,
            Func<Transform, UniTask<IRecipeItemView>> recipeItemViewGetter,
            Func<Transform, UniTask<IIngredientItemView>> ingredientItemViewGetter);
    }
}