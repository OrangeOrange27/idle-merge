using System;
using Features.Core.GridSystem.Tiles;
using UnityEngine;

namespace Features.Core
{
    public interface IPlaceableView
    {
        event Action OnTap;
        
        void SetModel(PlaceableModel model);
        void SetParentTile(IGameAreaTile tile);
        void Move(Vector3 position);
        void SetStage(int stage);
        
        void Select();
        void DeSelect();
    }
}