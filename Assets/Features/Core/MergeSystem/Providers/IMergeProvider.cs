using Features.Core.MergeSystem.Models;
using Features.Core.Placeables.Models;

namespace Features.Core.MergeSystem.Providers
{
    public interface IMergeProvider
    {
        PlaceableModel Get(MergeableType type, int stage);
    }
}