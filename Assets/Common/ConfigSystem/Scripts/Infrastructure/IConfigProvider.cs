using System;

namespace Common.Config.Infrastructure
{
	public interface IConfigProvider<T>
	{
		event Action OnUpdated;
		T Get();
	}
}
