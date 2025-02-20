using Microsoft.Extensions.Logging;
using Package.Logger.Abstraction;
using VContainer.Unity;

namespace DefaultNamespace
{
    public class GameServicesInstaller : LifetimeScope
    {
        private static readonly ILogger Logger = LogManager.GetLogger<GameServicesInstaller>();

    }
}