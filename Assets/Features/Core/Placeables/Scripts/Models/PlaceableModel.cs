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

        public PlaceableModel()
        {
        }

        protected PlaceableModel(PlaceableModel other)
        {
            ObjectType = other.ObjectType;
            ParentTile = new GameplayReactiveProperty<IGameAreaTile>(other.ParentTile.CurrentValue);
            Position = new GameplayReactiveProperty<Vector3>(other.Position.CurrentValue);
            IsSelected = new GameplayReactiveProperty<bool>(other.IsSelected.CurrentValue);
        }
        
        public virtual PlaceableModel Clone()
        {
            return new PlaceableModel(this);
        }

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