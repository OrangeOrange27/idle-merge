using Features.Core.MergeSystem.MergeableObjects;

namespace Features.Core.SupplySystem
{
    public interface ISupplyProvider
    {
        MergeableObject GetSupply();
    }
}