using VContainer;

namespace Features.Core.PlacementSystem
{
    public static class PlacementSystemRegistration
    {
        public static void RegisterPlacementSystem(this IContainerBuilder builder)
        {
            builder.Register<PlacementController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}