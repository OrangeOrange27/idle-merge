using System;
using System.Collections.Generic;
using Features.Core.Common.Models;
using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Factories;
using Features.Core.Placeables.Models;

namespace Features.Core.MergeSystem.Providers
{
    public class MergeProvider : IMergeProvider
    {
        private PlaceablesFactoryResolver _placeablesFactory;
        private readonly Dictionary<(MergeableType, int), PlaceableCreationInstruction> _mergesDictionary;

        public MergeProvider(MergesConfig mergesConfig, PlaceablesFactoryResolver placeablesFactory)
        {
            _placeablesFactory = placeablesFactory;
            _mergesDictionary = new Dictionary<(MergeableType, int), PlaceableCreationInstruction>();

            foreach (var mergeCfg in mergesConfig.Merges)
            {
                var key = (mergeCfg.RequiredType, mergeCfg.RequiredStage);
                if (_mergesDictionary.ContainsKey(key))
                    continue;

                _mergesDictionary.Add(key, mergeCfg.ResultObject);
            }
        }

        public PlaceableModel Get(MergeableType type, int stage)
        {
            _mergesDictionary.TryGetValue((type, stage), out var mergeResult);
            return GetModelFromMergeResult(mergeResult);
        }

        private PlaceableModel GetModelFromMergeResult(PlaceableCreationInstruction mergeResult)
        {
            Enum type = mergeResult.PlaceableType switch
            {
                PlaceableType.CollectibleObject => mergeResult.CollectibleType,
                PlaceableType.MergeableObject => mergeResult.MergeableType,
                PlaceableType.ProductionEntity => mergeResult.ProductionType,
                _ => throw new ArgumentOutOfRangeException()
            };

            var model = _placeablesFactory.Create(mergeResult.PlaceableType, type);
            if (model is MergeableModel mergeableModel)
                mergeableModel.Stage.Value = mergeResult.Stage;

            return model;
        }
    }
}