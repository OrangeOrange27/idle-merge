using System;
using Features.Core.GridSystem.Tiles;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Features.Core
{
    public class PlaceableView : MonoBehaviour, IPlaceableView, IPointerDownHandler
    {
        private static readonly Vector3 PositionOffset = new(0.5f, 0.5f, -1);

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
            tile.Occupy(_model);
            _model.Position.Value = tile.Position + PositionOffset;
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