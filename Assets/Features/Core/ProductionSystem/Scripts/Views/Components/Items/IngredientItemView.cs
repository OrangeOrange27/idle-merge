using Features.Core.Common.Views;
using TMPro;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public class IngredientItemView : CollectibleItemView, IIngredientItemView
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}