using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Features.Core.Grid.Managers
{
    public interface IPlaceablesVisualProvider
    {
        UniTask<IPlaceableView> Load(PlaceableModel model, IControllerResources controllerResources,
            CancellationToken cancellationToken, Transform parent = null);
    }
}