using System;
using Features.Core.GridSystem.Tiles;
using Features.Core.MergeSystem.Scripts.Models;

namespace Features.Core
{
    [Serializable]
    public class PlaceableModel
    {
        public int InternalId;
        public GameAreaObjectType ObjectType;
        public IPlaceableView View;
        
        public GameplayReactiveProperty<IGameAreaTile> ParentTile = new();

        //Mergeables
        public MergeableType MergeableType;
        public GameplayReactiveProperty<int> Stage = new();
    }
}