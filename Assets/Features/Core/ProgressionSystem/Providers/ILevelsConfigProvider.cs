using Features.Core.ProgressionSystem.Models;

namespace Features.Core.ProgressionSystem.Providers
{
    public interface ILevelsConfigProvider
    {
        LevelsConfig GetConfig();
    }
}