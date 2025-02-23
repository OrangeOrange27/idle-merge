using Features.Core.GridSystem;
using Package.Logger.Abstraction;
using UnityEngine;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Features.Core.SupplySystem
{
    public class SupplyManager : ISupplyManager
    {
        private static readonly ILogger Logger = LogManager.GetLogger<SupplyManager>();
        private static Vector3 Offset = new(0.5f, 0.5f, -1);
        
        private readonly GridManager _gridManager;
        private readonly ISupplyProvider _supplyProvider;

        public SupplyManager(GridManager gridManager, ISupplyProvider supplyProvider)
        {
            _gridManager = gridManager;
            _supplyProvider = supplyProvider;
        }

        public void SpawnSupply()
        {
            var freeTile = _gridManager.GetRandomFreeTile();
            if (freeTile == null)
                return;

            var position = freeTile.Position + Offset;
            var supply = Object.Instantiate(_supplyProvider.GetSupply(), position, Quaternion.identity);
            freeTile.Occupy(supply);
        }
    }
}