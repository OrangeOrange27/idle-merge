using System.Threading;
using Common.UI;
using Cysharp.Threading.Tasks;
using Features.DiscoveryBook.Scripts.Models;
using Features.DiscoveryBook.Scripts.Views;
using Package.AssetProvider.ViewLoader.Infrastructure;

namespace Features.DiscoveryBook.Scripts.States
{
    public class DiscoveryBookPopupState : BasicViewState<IDiscoveryBookPopupView, DiscoveryBookPopupPayload>
    {
        private readonly IViewLoader<IDiscoveryBookItemView, string> _itemViewLoader;
        private readonly IViewLoader<IDiscoveryBookSectionView> _sectionViewLoader;

        public DiscoveryBookPopupState(ISharedViewLoader<IDiscoveryBookPopupView> sharedViewLoader,
            IViewLoader<IDiscoveryBookItemView, string> itemViewLoader,
            IViewLoader<IDiscoveryBookSectionView> sectionViewLoader) : base(
            sharedViewLoader)
        {
            _itemViewLoader = itemViewLoader;
            _sectionViewLoader = sectionViewLoader;
        }

        protected override async UniTask SetInitialViewState(DiscoveryBookPopupPayload payload,
            IDiscoveryBookPopupView view,
            CancellationToken token)
        {
            await view.Initialize(payload.Tabs,
                async (container) =>
                    await _sectionViewLoader.Load(ControllerResources, token, container),
                async (key, container) =>
                    await _itemViewLoader.Load(key, ControllerResources, token, container),
                token);
        }

        protected override void SubscribeOnInput(IDiscoveryBookPopupView view)
        {
        }

        protected override void UnsubscribeOnInput(IDiscoveryBookPopupView view)
        {
        }
    }
}