using System;

namespace Common.Config.Infrastructure
{
	public interface IConfigProvider<out T>
	{
		event Action OnUpdated;
		T Get();
	}
}
