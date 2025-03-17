using Features.Core.ProductionSystem.Models;

namespace Features.Core.ProductionSystem.Providers
{
    public interface IRecycleConfigProvider
    {
        RecyclingConfig GetConfig();
    }
}