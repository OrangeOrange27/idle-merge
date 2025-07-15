using Features.Core.Placeables.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.Common.Views
{
    public class CollectibleItemView : ItemView, ICollectibleItemView
    {
        [SerializeField] private Image[] _icons;
        
        public void SetType(CollectibleType collectibleType)
        {
            for (var i = 0; i < _icons.Length; i++)
            {
                _icons[i].gameObject.SetActive(i + 1 == (int)collectibleType);
            }
        }
    }
}