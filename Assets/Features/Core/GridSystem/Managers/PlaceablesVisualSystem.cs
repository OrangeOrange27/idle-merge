using System;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Gameplay.Scripts.Controllers;
using Package.ControllersTree.Abstractions;

namespace Features.Core.GridSystem.Managers
{
    public class PlaceablesVisualSystem : IPlaceablesVisualSystem
    {
        private readonly Func<IPlaceableViewController> _viewControllerGetter;
        private readonly IPlaceablesVisualProvider _placeablesVisualProvider;
        private readonly IGameplayController _gameplayController;

        private IControllerResources _resources;
        private GameContext _gameContext;
        private IPlaceableViewController[] _viewControllers;

        public PlaceablesVisualSystem(Func<IPlaceableViewController> viewControllerGetter,
            IPlaceablesVisualProvider placeablesVisualProvider, IGameplayController gameplayController)
        {
            _viewControllerGetter = viewControllerGetter;
            _placeablesVisualProvider = placeablesVisualProvider;
            _gameplayController = gameplayController;
        }

        public async UniTask SpawnInitPlaceablesViews(GameContext context, IControllerResources resources,
            CancellationToken token)
        {
            _resources = resources;
            _gameContext = context;
            _gameContext.Placeables.CollectionChanged += OnPlaceablesCollectionChanged;

            _viewControllers =
                await UniTask.WhenAll(context.Placeables.Select(model => LoadSpawnView(model, resources, token)));
        }

        public async UniTask InitializePlaceablesViews()
        {
            foreach (var viewController in _viewControllers)
            {
                viewController.InitObserving();
            }
        }

        private void OnPlaceablesCollectionChanged(
            in ObservableCollections.NotifyCollectionChangedEventArgs<PlaceableModel> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.IsSingleItem)
                    {
                        LoadAndInitPlaceableView(e.NewItem).Forget();
                    }
                    else
                    {
                        foreach (var placeableModel in e.NewItems)
                        {
                            LoadAndInitPlaceableView(placeableModel).Forget();
                        }
                    }

                    break;
            }
        }

        private async UniTask LoadAndInitPlaceableView(PlaceableModel model)
        {
            await LoadSpawnView(model, _resources, CancellationToken.None)
                .ContinueWith(controller => controller.InitObserving());
        }

        private async UniTask<IPlaceableViewController> LoadSpawnView(PlaceableModel model,
            IControllerResources resources, CancellationToken token)
        {
            var view = await _placeablesVisualProvider.Load(model, resources, token,
                model.ParentTile.CurrentValue.Transform);
            var viewController = _viewControllerGetter.Invoke();

            model.View = view;
            viewController.InitOnCreate(_gameContext, view, model);
            resources.Attach(viewController);

            viewController.OnTap += OnViewTap;

            return viewController;
        }

        private void OnViewTap(PlaceableModel model)
        {
            _gameplayController.RegisterPlaceableClick(model);
        }
    }
}