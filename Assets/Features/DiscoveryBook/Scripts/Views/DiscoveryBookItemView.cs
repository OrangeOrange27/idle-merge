using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.DiscoveryBook.Scripts.Views
{
    public class DiscoveryBookItemView : MonoBehaviour, IDiscoveryBookItemView
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _text;
        
        public GameObject GameObject => gameObject;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}