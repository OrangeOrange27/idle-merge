using Features.Core.GridSystem;

namespace Features.Gameplay.View
{
    public interface IGameAreaView
    {
        IGridManager GridManager { get; }
    }
}