using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Models;
using UnityEngine;
using UnityEngine.EventSystems;
using PlaceableModel = Features.Core.Placeables.Models.PlaceableModel;

namespace Features.Core.Placeables.Views
{
    public class PlaceableView : MonoBehaviour, IPlaceableView, IPointerDownHandler
    {
        public event Action OnTap;

        protected PlaceableModel _model;
        private bool _isSelected;

        public bool CanMove => true;

        public void SetModel(PlaceableModel model)
        {
            _model = model;
        }

        public void Move(Vector3 position)
        {
            if (CanMove == false)
                return;

            transform.position = position;
        }

        public void Select()
        {
            _isSelected = true;
        }

        public void DeSelect()
        {
            _isSelected = false;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTap?.Invoke();
        }
        
        public virtual void SetStage(int stage)
        {
        }
        
        public virtual void Collect()
        {
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}