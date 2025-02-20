using System;
using UnityEngine.Assertions;
using UnityEngine.Scripting;
using Object = UnityEngine.Object;

namespace Package.Disposables
{
	public sealed class ObjectDisposable : IDisposable
	{
		private readonly Object _obj;

		[Preserve]
		public ObjectDisposable(Object obj)
		{
			Assert.IsTrue(obj);

			_obj = obj;
		}

		public void Dispose()
		{
			if (_obj)
				Object.Destroy(_obj);
		}
	}
}
