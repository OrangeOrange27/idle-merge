
using System;
using Common.UI;
using Common.Utils;
using Cysharp.Threading.Tasks;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.Placeables.Views
{
    public class ProductionObjectView : PlaceableView, IProductionObjectView
    {
        [SerializeField] private Tooltip _tooltip;

        private ProductionObjectModel Model => _model as ProductionObjectModel;
        
        public async UniTask ShowAndHideTooltip()
        {
            var timeToNextHarvest = Model.NextCollectionDateTime.Value - DateTime.Now;
            await _tooltip.ShowAndHideAsync(Helpers.FormatTimer(timeToNextHarvest));
        }

        public void ClaimProducts()
        {
        }

        public void Kill()
        {
        }
        
        
    }
}