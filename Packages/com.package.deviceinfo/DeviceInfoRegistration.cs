using VContainer;

namespace Common.DeviceInfo
{
	public static class DeviceInfoRegistration
	{
		public static void RegisterDeviceInfoRegistration(this IContainerBuilder builder)
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			builder.Register<StandaloneDeviceInfo>(Lifetime.Singleton).AsImplementedInterfaces();
#elif UNITY_ANDROID
			builder.Register<AndroidDeviceInfo>(Lifetime.Singleton).AsImplementedInterfaces();
#elif UNITY_IOS
			builder.Register<IosDeviceInfo>(Lifetime.Singleton).AsImplementedInterfaces();
#endif
		}
	}
}
