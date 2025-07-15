using Features.Core.Common.Views;
using Features.Core.Placeables.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.ProductionSystem.Components
{
    public class IngredientItemView : ItemView, IIngredientItemView
    {
        [SerializeField] private TMP_Text _text;
        [SerializeField] private Image[] _icons;
        
        public void SetIngredientType(CollectibleType collectibleType)
        {
            for (var i = 0; i < _icons.Length; i++)
            {
                _icons[i].gameObject.SetActive(i + 1 == (int)collectibleType);
            }
        }

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}