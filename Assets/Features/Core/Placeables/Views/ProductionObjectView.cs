
using Common.UI;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Features.Core.Placeables.Views
{
    public class ProductionObjectView : PlaceableView, IProductionObjectView
    {
        [SerializeField] private Tooltip _tooltip;
        
        public async UniTask ShowAndHideTooltip()
        {
            await _tooltip.ShowAndHideAsync();
        }

        public void ClaimProducts()
        {
            throw new System.NotImplementedException();
        }

        public void Kill()
        {
            throw new System.NotImplementedException();
        }
    }
}