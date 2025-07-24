using Features.Core.GameAreaInitializationSystem.Providers;
using VContainer;

namespace Features.Core.GameAreaInitializationSystem
{
    public static class GameAreaInitializationSystemRegistration
    {
        public static void RegisterGameAreaInitializationSystem(this IContainerBuilder builder, GameAreaConfigProvider gameAreaConfigProvider)
        {
            builder.RegisterInstance<IGameAreaConfigProvider, GameAreaConfigProvider>(gameAreaConfigProvider);
            builder.RegisterInstance(gameAreaConfigProvider.GetConfig());

            //builder.Register<IGameAreaInitializer, GameAreaInitializer>(Lifetime.Singleton);
            builder.Register<IGameAreaInitializer, TEMP_GameAreaInitializer>(Lifetime.Singleton); //todo: remove
        }
    }
}