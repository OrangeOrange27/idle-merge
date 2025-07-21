using System;
using Features.Core.MergeSystem.Models;

namespace Features.Core.SupplySystem.Models
{
    [Serializable]
    public class SupplyWeightsConfigEntry
    {
        public MergeableObjectConfig MergeableObject;
        public float Weight;
    }
    
    [Serializable]
    public class MergeableObjectConfig
    {
        public MergeableType MergeableType;
        public int Stage;
    }
    
    [Serializable]
    public class SupplyWeightsConfig
    {
        public SupplyWeightsConfigEntry[] WeightsArray;

        public static SupplyWeightsConfig Default => new()
        {
            WeightsArray = new SupplyWeightsConfigEntry[]
            {
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.BengalCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.MaineCoonCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.RagdollCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.SphynxCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.BritishShorthairCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.ScottishFoldCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.AbyssinianCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.RussianBlueCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.TabbyCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.SiameseCat,
                        Stage = 1
                    },
                    Weight = 1
                },
                new()
                {
                    MergeableObject = new MergeableObjectConfig()
                    {
                        MergeableType = MergeableType.PersianCat,
                        Stage = 1
                    },
                    Weight = 1
                }
            }
        };
    }
}