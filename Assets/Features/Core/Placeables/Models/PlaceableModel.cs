using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Views;
using UnityEngine;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class PlaceableModel : IDisposable
    {
        public PlaceableType ObjectType;
        public IPlaceableView View;
        
        public GameplayReactiveProperty<IGameAreaTile> ParentTile = new();
        public GameplayReactiveProperty<Vector3> Position = new();
        public GameplayReactiveProperty<bool> IsSelected = new();
        
        public bool IsDisposed { get; private set; }

        public virtual void Dispose()
        {
            IsDisposed = true;
            
            ParentTile.CurrentValue.DeOccupy();
            ParentTile?.Dispose();
            Position?.Dispose();
            IsSelected?.Dispose();
            
            View?.Dispose();
        }
    }
}