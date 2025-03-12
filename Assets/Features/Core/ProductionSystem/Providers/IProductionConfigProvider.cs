using Features.Core.MergeSystem.Models;
using Features.Core.ProductionSystem.Models;

namespace Features.Core.ProductionSystem.Providers
{
    public interface IProductionConfigProvider
    {
        ProductionsConfig GetConfig();
    }
}