using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.Placeables.Views;
using ObservableCollections;
using UnityEngine;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class PlaceableModel : IDisposable
    {
        public PlaceableType ObjectType;
        public IPlaceableView View;
        public Vector2 Size;

        public ObservableList<IGameAreaTile> OccupiedTiles = new();
        public GameplayReactiveProperty<Vector3> Position = new();
        public GameplayReactiveProperty<bool> IsSelected = new();

        public bool IsDisposed { get; private set; }

        public PlaceableModel()
        {
        }

        protected PlaceableModel(PlaceableModel other)
        {
            ObjectType = other.ObjectType;
            Size = other.Size;
            OccupiedTiles = new ObservableList<IGameAreaTile>(other.OccupiedTiles);
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

            foreach (var tile in OccupiedTiles)
            {
                tile.DeOccupy();
            }

            Position?.Dispose();
            IsSelected?.Dispose();

            View?.Dispose();
        }
    }
}