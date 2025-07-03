using System.Threading;
using Common.UI;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.ViewLoader.Infrastructure;

namespace Features.Core.ProductionSystem
{
    public class StartProductionState : BasicViewState<IProductionView>
    {
        public StartProductionState(ISharedViewLoader<IProductionView> sharedViewLoader) : base(sharedViewLoader)
        {
        }

        protected override void SubscribeOnInput(IProductionView view)
        {
            throw new System.NotImplementedException();
        }

        protected override void UnsubscribeOnInput(IProductionView view)
        {
            throw new System.NotImplementedException();
        }

        protected override UniTask SetInitialViewState(IProductionView view, CancellationToken token)
        {
            throw new System.NotImplementedException();
        }
    }
}