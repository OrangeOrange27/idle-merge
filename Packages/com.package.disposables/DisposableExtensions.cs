using System;
using Object = UnityEngine.Object;

namespace Package.Disposables
{
    /// <summary>
    /// Common Extensions for Disposable
    /// </summary>
    public static class DisposableExtensions
    {
        /// <summary>
        /// Converts game object to IDisposable that destroys the object on Dispose.
        /// </summary>
        public static IDisposable AsDisposable(this Object obj)
            => new ObjectDisposable(obj);
    }
}