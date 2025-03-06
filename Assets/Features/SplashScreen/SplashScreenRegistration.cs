using VContainer;

namespace Features.SplashScreen
{
    public static class SplashScreenRegistration
    {
        public static void RegisterSplashScreen(this IContainerBuilder builder, SplashSceneView splashSceneView)
        {
            if (splashSceneView != null)
            {
                builder.RegisterInstance(splashSceneView);
            }
            
            builder.Register<GameInitializationController>(Lifetime.Singleton).AsImplementedInterfaces();
        }
    }
}