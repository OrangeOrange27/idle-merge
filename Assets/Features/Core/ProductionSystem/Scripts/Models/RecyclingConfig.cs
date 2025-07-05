using System;
using Features.Core.Common.Models;
using Features.Core.Placeables.Models;

namespace Features.Core.ProductionSystem.Models
{
    [Serializable]
    public class RecyclingConfig
    {
        public RecyclingConfigEntry[] RecyclingConfigEntries;
    }

    [Serializable]
    public class RecyclingConfigEntry
    {
        public ProductionType ProductionType;
        public RecycleResult Config;
    }
    
    [Serializable]
    public class RecycleResult
    {
        public PlaceableCreationInstruction RecycleResultPlaceable;
        public int Amount;
    }
}