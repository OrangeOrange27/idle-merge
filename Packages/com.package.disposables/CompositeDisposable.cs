using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Package.Disposables
{
	public sealed class CompositeDisposable : IDisposable, ICollection<IDisposable>
	{
		private readonly List<IDisposable> _disposables = new List<IDisposable>();

		public void Add(IDisposable item)
		{
			if (item is null)
				throw new ArgumentNullException("item");

			_disposables.Add(item);
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

			bool shouldDispose;
			shouldDispose = _disposables.Remove(item);
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

		public void Dispose()
		{
			var exceptionList = new List<Exception>();

			foreach (var disposable in _disposables)
				try
				{
					disposable.Dispose();
				}
				catch (Exception e)
				{
					exceptionList.Add(e);
				}

			_disposables.Clear();
			if (exceptionList.Any()) throw new AggregateException(exceptionList);
		}
	}
}
