using System;
using Features.Core.Common.Views;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Components
{
    public interface IRecipeComponentView : IItemView
    {
        event Action OnHintButtonPressedEvent;
        void SetCollectibleType(CollectibleType collectibleType);
    }
}