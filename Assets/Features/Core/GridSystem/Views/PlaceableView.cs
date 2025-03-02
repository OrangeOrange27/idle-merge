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
        
        public void SetModel(PlaceableModel model)
        {
            _model = model;
        }

        public void SetParentTile(IGameAreaTile tile)
        {
            transform.position = tile.Position + PositionOffset;
            tile.Occupy(_model);
        }

        public virtual void SetStage(int stage)
        {
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            OnTap?.Invoke();
        }
    }
}