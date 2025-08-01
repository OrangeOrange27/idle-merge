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
        public DiscoveryBookPopupState(ISharedViewLoader<IDiscoveryBookPopupView> sharedViewLoader) : base(
            sharedViewLoader)
        {
        }

        protected override UniTask SetInitialViewState(DiscoveryBookPopupPayload payload, IDiscoveryBookPopupView view,
            CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        protected override void SubscribeOnInput(IDiscoveryBookPopupView view)
        {
            throw new System.NotImplementedException();
        }

        protected override void UnsubscribeOnInput(IDiscoveryBookPopupView view)
        {
            throw new System.NotImplementedException();
        }
    }
}