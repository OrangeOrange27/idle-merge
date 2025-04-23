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
        [SerializeField] private Tooltip _timerTooltip;
        [SerializeField] private Tooltip _harvestTooltip;

        private ProductionObjectModel Model => _model as ProductionObjectModel;
        private TimeSpan TimeToNextHarvest => Model.NextCollectionDateTime.Value - DateTime.Now;

        public void ShowHarvestTooltip()
        {
            _harvestTooltip.Show(Helpers.FormatTimer(TimeToNextHarvest));
        }

        public async UniTask HideHarvestTooltip()
        {
            await _harvestTooltip.HideAsync();
        }

        public async UniTask ShowAndHideTimerTooltip()
        {
            await _timerTooltip.ShowAndHideAsync(Helpers.FormatTimer(TimeToNextHarvest));
        }

        public void ClaimProducts()
        {
        }

        public void Kill()
        {
        }
    }
}