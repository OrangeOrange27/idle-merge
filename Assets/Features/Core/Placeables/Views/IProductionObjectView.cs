using Cysharp.Threading.Tasks;

namespace Features.Core.Placeables.Views
{
    public interface IProductionObjectView
    {
        UniTask ShowAndHideTooltip();
        void ClaimProducts();
        void Kill();
    }
}