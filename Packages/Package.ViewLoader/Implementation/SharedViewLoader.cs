using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Package.AssetProvider.ViewLoader.Infrastructure;
using Package.Disposables;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Implementation
{
	internal class SharedViewLoader<TViewInterface, TKey> : ISharedViewLoader<TViewInterface>, ISharedViewLoader<TViewInterface, TKey> where TViewInterface : class
	{
		private readonly IViewLoader<TViewInterface, TKey> _viewLoader;

		private readonly Dictionary<TKey,TViewInterface> _cachedValue = new Dictionary<TKey, TViewInterface>();
		private readonly Dictionary<TKey, CompositeDisposable> _compositeDisposable = new Dictionary<TKey, CompositeDisposable>();
		private readonly Dictionary<TKey,int> _referenceCount = new Dictionary<TKey,int>();

		public SharedViewLoader(IViewLoader<TViewInterface, TKey> viewLoader)
		{
			_viewLoader = viewLoader;
		}

		public UniTask Prewarm(ICollection<IDisposable> resources, CancellationToken cancellationToken)
		{
			return _viewLoader.Prewarm(default, resources, cancellationToken, 1);
		}

		public async UniTask<object> LoadDefault(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent)
		{
			return await Load(resources, cancellationToken, parent);
		}
		
		public async UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent)
		{
			_referenceCount.TryAdd(key, 0);
			_referenceCount[key]++;
			var disposable = new Disposable(() =>
			{
				_referenceCount[key]--;
				DisposeLoaderIfReferenceCountZero(key);
			});
			resources.Add(disposable);

			if (_cachedValue.ContainsKey(key))
				return _cachedValue[key];

			if(!_compositeDisposable.ContainsKey(key))
				_compositeDisposable.Add(key, new CompositeDisposable());
			_cachedValue[key] = await _viewLoader.Load(key, _compositeDisposable[key], cancellationToken, parent);

			return _cachedValue[key];
		}

		public TViewInterface CachedView => _cachedValue.FirstOrDefault().Value;

		public async UniTask<TViewInterface> Load(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent)
		{
			return await Load(default, resources, cancellationToken, parent);
		}

		private void DisposeLoaderIfReferenceCountZero(TKey key)
		{
			if (_referenceCount[key] > 0)
			{
				return;
			}

			_compositeDisposable[key].Dispose();
			_compositeDisposable[key] = new CompositeDisposable(); //it's possible to reuse but it not obvious to continue using disposed object
			_referenceCount[key] = 0;
			_cachedValue.Remove(key);
		}
	}
}
