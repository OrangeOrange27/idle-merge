using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.MergeSystem.Scripts.Models;
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

        //Mergeables
        public MergeableType MergeableType;
        public GameplayReactiveProperty<int> Stage = new();
        
        //Collectibles
        public CollectibleType CollectibleType;
        
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            IsDisposed = true;
            
            ParentTile.CurrentValue.DeOccupy();
            ParentTile?.Dispose();
            Position?.Dispose();
            IsSelected?.Dispose();
            Stage?.Dispose();
            
            View?.Dispose();
        }
    }
    
    public static class PlaceablesExtensions
    {
        public static bool CanMergeWith(this PlaceableModel original, PlaceableModel other)
        {
            return original.MergeableType == other.MergeableType && original.Stage.Value == other.Stage.Value;
        }
    }
}