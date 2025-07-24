using Features.Core.Common.Views;

namespace Features.Core.ProductionSystem.Components
{
    public interface IMergeableItemView : IItemView
    {
        void SetStage(int stage);
    }
}