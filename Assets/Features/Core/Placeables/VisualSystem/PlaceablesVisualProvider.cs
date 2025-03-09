using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Models;
using Features.Core.Placeables.Views;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.ControllersTree.Abstractions;
using UnityEngine;

namespace Features.Core.Placeables.VisualSystem
{
    public class PlaceablesVisualProvider : IPlaceablesVisualProvider
    {
        private readonly IViewLoader<IPlaceableView, string> _defaultViewLoader;
        private readonly IViewLoader<IPlaceableView, MergeableType> _mergeableViewLoader;

        public PlaceablesVisualProvider(IViewLoader<IPlaceableView, string> defaultViewLoader,
            IViewLoader<IPlaceableView, MergeableType> mergeableViewLoader)
        {
            _defaultViewLoader = defaultViewLoader;
            _mergeableViewLoader = mergeableViewLoader;
        }

        public UniTask<IPlaceableView> Load(PlaceableModel model, IControllerResources controllerResources,
            CancellationToken cancellationToken, Transform parent = null)
        {
            return model.ObjectType switch
            {
                PlaceableType.MergeableObject => _mergeableViewLoader.Load(model.MergeableType,
                    controllerResources,
                    cancellationToken, parent),
                _ => _defaultViewLoader.Load(model.ObjectType.ToString(), controllerResources, cancellationToken,
                    parent)
            };
        }
    }
}