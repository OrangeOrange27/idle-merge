using Features.Core.GameAreaInitializationSystem.Models;

namespace Features.Core.GameAreaInitializationSystem.Providers
{
    public interface IGameAreaConfigProvider
    {
        GameAreaConfig GetConfig();
    }
}