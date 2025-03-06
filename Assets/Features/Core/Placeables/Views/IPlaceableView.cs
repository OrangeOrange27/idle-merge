using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Models;
using UnityEngine;

namespace Features.Core.Placeables.Views
{
    public interface IPlaceableView : IDisposable
    {
        event Action OnTap;
        
        void SetModel(PlaceableModel model);
        void SetParentTile(IGameAreaTile tile);
        void Move(Vector3 position);
        
        //Mergeables
        void SetStage(int stage);
        
        //Collectibles
        void Collect();
        
        void Select();
        void DeSelect();
    }
}