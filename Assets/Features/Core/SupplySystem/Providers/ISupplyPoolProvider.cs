using System.Collections.Generic;
using Common.Data;
using Features.Core.Placeables.Models;

namespace Features.Core.SupplySystem.Providers
{
    public interface ISupplyPoolProvider
    {
        List<WeightedEntry<PlaceableModel>> GetSpawnPool();
    }
}