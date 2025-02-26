using Features.Core.GridSystem;
using Features.Gameplay.View;
using Package.AssetProvider.ViewLoader.Infrastructure;
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
        private readonly ISharedViewLoader<IGameView> _gameViewLoader;
        
        private IGridManager _gridManager;
        private IGridManager GridManager => _gridManager ??= _gameViewLoader.CachedView.GameAreaView.GridManager;

        public SupplyManager(ISharedViewLoader<IGameView> gameViewLoader, ISupplyProvider supplyProvider)
        {
            _gameViewLoader = gameViewLoader;
            _supplyProvider = supplyProvider;
        }

        public void SpawnSupply()
        {
            var freeTile = GridManager.GetRandomFreeTile();
            if (freeTile == null)
                return;

            var position = freeTile.Position + Offset;
            var supply = Object.Instantiate(_supplyProvider.GetSupply(), position, Quaternion.identity);
            freeTile.Occupy(supply);
        }
    }
}