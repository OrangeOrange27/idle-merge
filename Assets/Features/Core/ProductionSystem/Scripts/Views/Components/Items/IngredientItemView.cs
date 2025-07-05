using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using TMPro;
using UnityEngine;

namespace Features.Core.ProductionSystem.Components
{
    public class IngredientItemView : ItemView, IIngredientItemView
    {
        [SerializeField] private TMP_Text _text;
        
        public void SetIngredientType(CollectibleType collectibleType)
        {
            throw new System.NotImplementedException();
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}