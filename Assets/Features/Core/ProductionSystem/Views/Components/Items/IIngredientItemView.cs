using Features.Core.Common.Views;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Components
{
    public interface IIngredientItemView : IItemView
    {
        void SetIngredientType(CollectibleType collectibleType);
    }
}