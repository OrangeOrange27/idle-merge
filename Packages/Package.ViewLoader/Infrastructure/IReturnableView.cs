using System;
using System.Collections.Generic;

namespace Package.AssetProvider.Infrastructure
{
	public interface IReturnableView<T> : ICollection<IDisposable>, IDisposable
	{
		T Get();
		IReturnableView<T> Set(T value);
	}
}
