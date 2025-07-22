using System;
using Common.Config.Infrastructure;
using Features.Core.Common.Models;
using Features.Core.Placeables.Models;

namespace Features.Core.MergeSystem.Models
{
    [Serializable]
    public class MergesConfig : BaseConfig
    {
        public MergesConfigEntry[] Merges;
    }
    
    [Serializable]
    public class MergesConfigEntry
    {
        public MergeableType RequiredType;
        public int RequiredStage;
        public PlaceableCreationInstruction ResultObject;
    }
}