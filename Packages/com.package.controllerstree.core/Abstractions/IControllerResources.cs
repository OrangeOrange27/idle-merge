using System;
using System.Collections.Generic;

namespace Package.ControllersTree.Abstractions
{
    public interface IControllerResources : IDisposable, ICollection<IDisposable>
    {
        void Attach(IDisposable disposable);
    }
}