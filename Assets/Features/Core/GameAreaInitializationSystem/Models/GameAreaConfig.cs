using System;
using System.Collections.Generic;
using Features.Core.Placeables.Models;
using Package.Logger.Abstraction;
using UnityEngine;
using ZLogger;

namespace Features.Core.GameAreaInitializationSystem.Models
{
    [Serializable]
    public class GameAreaPlaceableConfigEntry
    {
        public Vector3Int Position;
        public PlaceableModel Placeable;
    }

    [Serializable]
    public class GameAreaConfig : ISerializationCallbackReceiver
    {
        private static readonly Microsoft.Extensions.Logging.ILogger Logger = LogManager.GetLogger<GameAreaConfig>();

        [SerializeField] private List<GameAreaPlaceableConfigEntry> _placeableConfigs = new();

        public Dictionary<Vector3Int, PlaceableModel> Placeables { get; private set; }

        public GameAreaConfig()
        {
            Placeables = new Dictionary<Vector3Int, PlaceableModel>();
        }
        public GameAreaConfig(List<GameAreaPlaceableConfigEntry> placeableConfigs)
        {
            _placeableConfigs = placeableConfigs;
            OnAfterDeserialize();
        }

        public void OnBeforeSerialize()
        {
            _placeableConfigs.Clear();

            foreach (var cfg in Placeables)
            {
                _placeableConfigs.Add(new GameAreaPlaceableConfigEntry { Position = cfg.Key, Placeable = cfg.Value });
            }
        }

        public void OnAfterDeserialize()
        {
            Placeables = new Dictionary<Vector3Int, PlaceableModel>();

            foreach (var cfg in _placeableConfigs)
            {
                if (!Placeables.TryAdd(cfg.Position, cfg.Placeable))
                {
                    Logger.ZLogError($"Multiple entries for tile: {cfg.Position}");
                }
            }
        }
    }
}