using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.Placeables.Models;
using Features.Core.Placeables.Views;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Features.Core.Placeables.VisualSystem
{
    public interface IPlaceablesVisualProvider
    {
        UniTask<IPlaceableView> Load(PlaceableModel model, IControllerResources controllerResources,
            CancellationToken cancellationToken, Transform parent = null);
    }
}