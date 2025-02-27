using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.MergeSystem.Scripts.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Features.Core.Grid.Managers
{
    public class PlaceablesVisualProvider : IPlaceablesVisualProvider
    {
        private readonly IViewLoader<IPlaceableView, string> _defaultViewLoader;
        private readonly IViewLoader<IPlaceableView, MergeableType> _mergeableViewLoader;

        public UniTask<IPlaceableView> Load(PlaceableModel model, IControllerResources controllerResources,
            CancellationToken cancellationToken, Transform parent = null)
        {
            return model.ObjectType switch
            {
                GameAreaObjectType.MergeableObject => _mergeableViewLoader.Load(model.MergeableType, controllerResources,
                    cancellationToken, parent),
                _ => _defaultViewLoader.Load(model.ObjectType.ToString(), controllerResources, cancellationToken,
                    parent)
            };
        }
    }
}