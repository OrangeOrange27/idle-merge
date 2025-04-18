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
        private readonly IViewLoader<IPlaceableView, CollectibleType> _collectibleViewLoader;
        private readonly IViewLoader<IPlaceableView, ProductionType> _productionEntityViewLoader;

        public PlaceablesVisualProvider(IViewLoader<IPlaceableView, string> defaultViewLoader,
            IViewLoader<IPlaceableView, MergeableType> mergeableViewLoader,
            IViewLoader<IPlaceableView, CollectibleType> collectibleViewLoader,
            IViewLoader<IPlaceableView, ProductionType> productionEntityViewLoader)
        {
            _defaultViewLoader = defaultViewLoader;
            _mergeableViewLoader = mergeableViewLoader;
            _collectibleViewLoader = collectibleViewLoader;
            _productionEntityViewLoader = productionEntityViewLoader;
        }

        public UniTask<IPlaceableView> Load(PlaceableModel model, IControllerResources controllerResources,
            CancellationToken cancellationToken, Transform parent = null)
        {
            return model switch
            {
                MergeableModel mergeableModel => _mergeableViewLoader.Load(mergeableModel.MergeableType,
                    controllerResources, cancellationToken, parent),
                CollectibleModel collectibleModel => _collectibleViewLoader.Load(
                    collectibleModel.CollectibleType, controllerResources, cancellationToken, parent),
                ProductionObjectModel productionObjectModel => _productionEntityViewLoader.Load(
                    productionObjectModel.ProductionType, controllerResources, cancellationToken, parent),
                _ => _defaultViewLoader.Load(model.ObjectType.ToString(), controllerResources, cancellationToken,
                    parent)
            };
        }
    }
}