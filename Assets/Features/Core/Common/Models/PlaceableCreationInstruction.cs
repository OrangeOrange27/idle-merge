using System;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Models;
using UnityEngine.Serialization;

namespace Features.Core.Common.Models
{
    [Serializable]
    public class PlaceableCreationInstruction
    {
        public PlaceableType PlaceableType;
        public MergeableType MergeableType;
        public int Stage;
        public CollectibleType CollectibleType;
        public ProductionType ProductionType;
    }
}