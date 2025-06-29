using System;
using Features.Core.SupplySystem.Providers;
using Features.Gameplay.View;
using Package.Logger.Abstraction;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.SupplySystem
{
    public class SupplyManager : ISupplyManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger<SupplyManager>();
        private static Vector3 Offset = new(0.5f, 0.5f, -1);
        
        private readonly ISupplyProvider _supplyProvider;
        private readonly Func<IGameView> _gameViewGetter;
        
        public SupplyManager(Func<IGameView> gameViewGetter, ISupplyProvider supplyProvider)
        {
            _gameViewGetter = gameViewGetter;
            _supplyProvider = supplyProvider;
        }

        public void SpawnSupply(GameContext gameContext)
        {
            var freeTile = _gameViewGetter.Invoke().GameAreaView.GridManager.GetRandomFreeTile();
            if (freeTile == null)
                return;

            var supply = _supplyProvider.GetSupply();
            supply.OccupiedTiles.Add(freeTile);
            gameContext.Placeables.Add(supply);
        }
    }
}