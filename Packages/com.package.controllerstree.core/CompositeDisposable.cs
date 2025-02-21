using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Package.ControllersTree
{
    public class CompositeDisposable : IDisposable, ICollection<IDisposable>
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private bool _isDisposed;

        public void Add(IDisposable item)
        {
            if (item is null)
                throw new ArgumentNullException("item");

            if (!_isDisposed)
            {
                _disposables.Add(item);
            }
            else
            {
                item.Dispose();
            }
        }

        public void Clear()
        {
            Dispose();
        }

        public bool Contains(IDisposable item)
        {
            return _disposables.Contains(item);
        }

        public void CopyTo(IDisposable[] array, int arrayIndex)
        {
            _disposables.CopyTo(array, arrayIndex);
        }

        public bool Remove(IDisposable item)
        {
            if (item is null)
                throw new ArgumentNullException("item");

            var shouldDispose = false;
            if (!_isDisposed)
            {
                shouldDispose = _disposables.Remove(item);
            }

            if (shouldDispose)
                item.Dispose();

            return shouldDispose;
        }

        public int Count => _disposables.Count;

        public bool IsReadOnly => false;


        public IEnumerator<IDisposable> GetEnumerator()
        {
            return _disposables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_disposables).GetEnumerator();
        }

        private void ReleaseUnmanagedResources()
        {
            List<Exception> exceptionList = null;

            foreach (var disposable in _disposables)
            {
                try
                {
                    disposable.Dispose();
                }
                catch (Exception e)
                {
                    if (exceptionList == null) exceptionList = new List<Exception>();
                    exceptionList.Add(e);
                }
            }

            _disposables.Clear();
            if (exceptionList != null && exceptionList.Any())
            {
                throw new AggregateException(exceptionList);
            }

            _isDisposed = true;
        }

        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        ~CompositeDisposable()
        {
            ReleaseUnmanagedResources();
        }
    }
}