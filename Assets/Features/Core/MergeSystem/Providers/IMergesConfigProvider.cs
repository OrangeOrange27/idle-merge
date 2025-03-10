using Features.Core.MergeSystem.Models;

namespace Features.Core.MergeSystem.Providers
{
    public interface IMergesConfigProvider
    {
        MergesConfig GetConfig();
    }
}