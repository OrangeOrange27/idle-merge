using System.Collections.Generic;
using System.Linq;
using Common.Data;
using Common.Utils.Extensions;
using Features.Core.Placeables.Models;
using Random = UnityEngine.Random;

namespace Features.Core.SupplySystem.Providers
{
    public class RandomSupplyProvider : ISupplyProvider
    {
        private readonly ISupplyPoolProvider _supplyPoolProvider;
        private List<WeightedEntry<PlaceableModel>> _supplyPool;

        public RandomSupplyProvider(ISupplyPoolProvider supplyPoolProvider)
        {
            _supplyPoolProvider = supplyPoolProvider;
        }

        public PlaceableModel GetSupply()
        {
            return GetRandomSupply();
        }

        private PlaceableModel GetRandomSupply()
        {
            if (_supplyPool.IsNullOrEmpty())
                _supplyPool = _supplyPoolProvider.GetSpawnPool();

            var totalWeight = _supplyPool.Sum(entry => entry.Weight);
            var rnd = Random.Range(0f, totalWeight);
            var cumulativeWeight = 0f;

            foreach (var entry in _supplyPool)
            {
                cumulativeWeight += entry.Weight;

                if (rnd <= cumulativeWeight)
                {
                    return entry.Item.Clone();
                }
            }

            return _supplyPool[^1].Item.Clone();
        }
    }
}