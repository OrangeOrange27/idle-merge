using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Features.Core.GridSystem.Managers
{
    public interface IPlaceablesVisualSystem
    {
        UniTask SpawnInitPlaceablesViews(GameContext context, IControllerResources resources, CancellationToken token);
        UniTask InitializePlaceablesViews();
    }
}