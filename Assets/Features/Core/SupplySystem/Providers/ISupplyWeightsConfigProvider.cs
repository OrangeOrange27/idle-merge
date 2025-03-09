using Features.Core.SupplySystem.Config;

namespace Features.Core.SupplySystem.Providers
{
    public interface ISupplyWeightsConfigProvider
    {
        SupplyWeightsConfig GetConfig();
    }
}