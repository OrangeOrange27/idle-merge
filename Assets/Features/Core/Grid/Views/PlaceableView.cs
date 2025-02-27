using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Core
{
    public class PlaceableView : MonoBehaviour, IPlaceableView, IPointerDownHandler
    {
        public event Action OnTap;
        
        private PlaceableModel _model;
        
        public void SetModel(PlaceableModel model)
        {
            _model = model;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTap?.Invoke();
        }
    }
}