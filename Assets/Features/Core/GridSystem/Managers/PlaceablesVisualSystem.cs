using System;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.ControllersTree.Abstractions;

namespace Features.Core.GridSystem.Managers
{
    public class PlaceablesVisualSystem : IPlaceablesVisualSystem
    {
        private readonly Func<IPlaceableViewController> _cardViewControllerGetter;
        private readonly IPlaceablesVisualProvider _placeablesVisualProvider;

        private IControllerResources _resources;
        private GameContext _gameContext;
        private IPlaceableViewController[] _viewControllers;

        public PlaceablesVisualSystem(Func<IPlaceableViewController> cardViewControllerGetter,
            IPlaceablesVisualProvider placeablesVisualProvider)
        {
            _cardViewControllerGetter = cardViewControllerGetter;
            _placeablesVisualProvider = placeablesVisualProvider;
        }

        public async UniTask SpawnInitPlaceablesViews(GameContext context, IControllerResources resources,
            CancellationToken token)
        {
            _resources = resources;
            _gameContext = context;
            _gameContext.Placeables.CollectionChanged += CardsOnCollectionChanged;

            _viewControllers =
                await UniTask.WhenAll(context.Placeables.Select(model => LoadSpawnView(model, resources, token)));
        }

        public async UniTask InitializePlaceablesViews()
        {
            foreach (var cardViewController in _viewControllers)
            {
                cardViewController.InitObserving();
            }
        }

        private void CardsOnCollectionChanged(
            in ObservableCollections.NotifyCollectionChangedEventArgs<PlaceableModel> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.IsSingleItem)
                    {
                        LoadSpawnView(e.NewItem, _resources, CancellationToken.None)
                            .ContinueWith(controller => controller.InitObserving())
                            .Forget();
                    }
                    else
                    {
                        foreach (var cardModel in e.NewItems)
                        {
                            LoadSpawnView(cardModel, _resources, CancellationToken.None)
                                .ContinueWith(controller => controller.InitObserving())
                                .Forget();
                        }
                    }

                    break;
            }
        }

        private async UniTask<IPlaceableViewController> LoadSpawnView(PlaceableModel model,
            IControllerResources resources, CancellationToken token)
        {
            var view = await _placeablesVisualProvider.Load(model, resources, token,
                model.ParentTile.CurrentValue.Transform);
            var cardViewController = _cardViewControllerGetter.Invoke();

            model.View = view;
            cardViewController.InitOnCreate(_gameContext, view, model);
            resources.Attach(cardViewController);

            cardViewController.OnTap += CardViewOnTap;

            return cardViewController;
        }

        private void CardViewOnTap(PlaceableModel model)
        {
            throw new NotImplementedException();
        }
    }
}