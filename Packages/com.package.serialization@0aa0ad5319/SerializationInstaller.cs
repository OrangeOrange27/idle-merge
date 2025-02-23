using Common.Serialization.Infrastructure;
using VContainer;
using JsonSerializer = Common.Serialization.Implementation.JsonSerializer;

namespace Common.Serialization
{
	public static class SerializationInstaller 
	{
		public static void RegisterSerialization(this IContainerBuilder container)
		{
			container.Register<ISerializer, JsonSerializer>(Lifetime.Singleton);
		}
	}
}