using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.MergeSystem.Models;
using UnityEngine;

namespace Features.Core.Placeables.Models
{
    [Serializable]
    public class MergeableModel : PlaceableModel
    {
        public MergeableType MergeableType;
        public GameplayReactiveProperty<int> Stage = new();
        
        public MergeableModel()
        {
        }

        protected MergeableModel(MergeableModel other) : base(other)
        {
            MergeableType = other.MergeableType;
            Stage = new GameplayReactiveProperty<int>(other.Stage.Value);
        }
        
        public override PlaceableModel Clone()
        {
            return new MergeableModel(this);
        }

        public override void Dispose()
        {
            base.Dispose();
            Stage?.Dispose();
        }
    }
    
    public static class MergeablesExtensions
    {
        public static bool CanMergeWith(this MergeableModel original, MergeableModel other)
        {
            if(original == null || other == null)
                return false;
            
            return original.MergeableType == other.MergeableType && original.Stage.Value == other.Stage.Value;
        }
        
        public static bool CanMergeWith(this MergeableModel original, PlaceableModel other)
        {
            if(original == null || other is not MergeableModel otherMergeable)
                return false;
            
            return original.MergeableType == otherMergeable.MergeableType && original.Stage.Value == otherMergeable.Stage.Value;
        }
    }
}