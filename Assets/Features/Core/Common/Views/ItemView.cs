using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Features.Core.Common.Views
{
    public class ItemView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _text;

        public virtual void SetText(string text)
        {
            _text.text = text;
        }
    }
}