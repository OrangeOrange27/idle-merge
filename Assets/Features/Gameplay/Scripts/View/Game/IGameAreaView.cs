using Features.Core.GridSystem;
using Features.Core.GridSystem.Managers;

namespace Features.Gameplay.View
{
    public interface IGameAreaView
    {
        IGridManager GridManager { get; }
    }
}