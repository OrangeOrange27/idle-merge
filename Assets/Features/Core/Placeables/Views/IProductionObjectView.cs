using Cysharp.Threading.Tasks;

namespace Features.Core.Placeables.Views
{
    public interface IProductionObjectView
    {
        void ShowHarvestTooltip();
        UniTask HideHarvestTooltip();
        UniTask ShowAndHideTimerTooltip();
        void ClaimProducts();
        void Kill();
    }
}