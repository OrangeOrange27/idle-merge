using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Features.Core.Grid.Managers
{
    public interface IPlaceablesVisualSystem
    {
        UniTask SpawnInitPlaceablesViews(GameContext context, IControllerResources resources);
        UniTask InitializePlaceablesViews();
    }
}