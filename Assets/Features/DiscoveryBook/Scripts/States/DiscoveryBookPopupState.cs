using System.Threading;
using Common.UI;
using Cysharp.Threading.Tasks;
using Features.Core.ProductionSystem;
using Features.DiscoveryBook.Scripts.Models;
using Package.AssetProvider.ViewLoader.Infrastructure;

namespace Features.DiscoveryBook.Scripts.States
{
    public class DiscoveryBookPopupState : BasicViewState<IProductionView, DiscoveryBookPopupPayload>
    {
        public DiscoveryBookPopupState(ISharedViewLoader<IProductionView> sharedViewLoader) : base(sharedViewLoader)
        {
        }

        protected override UniTask SetInitialViewState(DiscoveryBookPopupPayload payload, IProductionView view, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }

        protected override void SubscribeOnInput(IProductionView view)
        {
            throw new System.NotImplementedException();
        }

        protected override void UnsubscribeOnInput(IProductionView view)
        {
            throw new System.NotImplementedException();
        }
    }
}