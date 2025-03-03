using VContainer;

namespace Features.Core.PlacementSystem
{
    public static class PlacementSystemRegistration
    {
        public static void RegisterPlacementSystem(this IContainerBuilder builder)
        {
            builder.Register<SelectionController>(Lifetime.Singleton).AsImplementedInterfaces();
            builder.Register<PlacementSystem>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}