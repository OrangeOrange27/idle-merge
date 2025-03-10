using Features.Core.SupplySystem.Models;

namespace Features.Core.SupplySystem.Providers
{
    public interface ISupplyWeightsConfigProvider
    {
        SupplyWeightsConfig GetConfig();
    }
}