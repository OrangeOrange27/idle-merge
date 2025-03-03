using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.PlacementSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Core
{
    public class PlaceableView : MonoBehaviour, IPlaceableView, IPointerDownHandler
    {
        public event Action OnTap;

        private PlaceableModel _model;
        private bool _isSelected;

        public bool CanMove => true;

        public void SetModel(PlaceableModel model)
        {
            _model = model;
        }

        public void SetParentTile(IGameAreaTile tile)
        {
            _model.Position.Value = tile.Position + Constants.PlaceableOffset;
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
    }
}