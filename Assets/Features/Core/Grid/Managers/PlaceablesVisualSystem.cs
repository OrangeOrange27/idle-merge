using System;
using System.Collections.Specialized;
using System.Threading;
using Cysharp.Threading.Tasks;
using Features.Core.Grid.Managers;
using Package.ControllersTree.Abstractions;

namespace Features.Core
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

        public async UniTask SpawnInitPlaceablesViews(GameContext context, IControllerResources resources)
        {
            _resources = resources;
            _gameContext = context;
            _gameContext.Placeables.CollectionChanged += CardsOnCollectionChanged;

            _viewControllers = await UniTask.WhenAll(context.Placeables.Select(model => LoadSpawnView(model, resources)));
        }
        
        public async UniTask InitializePlaceablesViews()
        {
            foreach (var cardViewController in _viewControllers)
            {
                cardViewController.InitObserving();
            }
        }

        private void CardsOnCollectionChanged(in ObservableCollections.NotifyCollectionChangedEventArgs<PlaceableModel> e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.IsSingleItem)
                    {
                        LoadSpawnView(e.NewItem, _resources).ContinueWith(controller => controller.InitObserving())
                            .Forget();
                    }
                    else
                    {
                        foreach (var cardModel in e.NewItems)
                        {
                            LoadSpawnView(cardModel, _resources).ContinueWith(controller => controller.InitObserving())
                                .Forget();
                        }
                    }

                    break;
            }
        }

        private async UniTask<IPlaceableViewController> LoadSpawnView(PlaceableModel model, IControllerResources resources)
        {
            var view = await _placeablesVisualProvider.Load(model, resources, CancellationToken.None,
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