using System;
using Package.ControllersTree.Abstractions;

namespace Package.ControllersTree.Implementations
{
	public class ControllerResources : CompositeDisposable, IControllerResources
	{
		public void Attach(IDisposable disposable)
		{
			Add(disposable);
		}
	}
}
