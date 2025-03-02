using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.MergeSystem.Scripts.Models;
using UnityEngine;

namespace Features.Core
{
    [Serializable]
    public class PlaceableModel
    {
        public GameAreaObjectType ObjectType;
        public IPlaceableView View;
        
        public GameplayReactiveProperty<IGameAreaTile> ParentTile = new();
        public GameplayReactiveProperty<Vector3> Position = new();
        public GameplayReactiveProperty<bool> IsSelected = new();

        //Mergeables
        public MergeableType MergeableType;
        public GameplayReactiveProperty<int> Stage = new();
    }
}