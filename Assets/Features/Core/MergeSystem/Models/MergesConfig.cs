using System;
using Features.Core.Placeables.Models;

namespace Features.Core.MergeSystem.Models
{
    [Serializable]
    public class MergesConfig
    {
        public MergesConfigEntry[] Merges;
    }
    
    [Serializable]
    public class MergesConfigEntry
    {
        public MergeableType RequiredType;
        public int RequiredStage;
        public MergeResult ResultObject;
    }

    //don't really like it :/
    [Serializable]
    public class MergeResult
    {
        public PlaceableType ObjectType;
        public MergeableType MergeableType;
        public int Stage;
        public CollectibleType CollectibleType;
        public CollectibleType ProductionType;
    }
}