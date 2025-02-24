using System;
using System.Collections.Generic;
using Package.ControllersTree;

namespace Package.AssetProvider.Infrastructure
{
	public class ReturnableView<T> : CompositeDisposable, IReturnableView<T>
	{
		private T _instance;

		public ReturnableView(ICollection<IDisposable> controllerResourcesLinkeage)
		{
			controllerResourcesLinkeage.Add(this);
		}

		public T Get()
		{
			return _instance;
		}

		public IReturnableView<T> Set(T value)
		{
			_instance = value;
			return this;
		}
	}
}
