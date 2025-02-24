using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Infrastructure
{
	public interface ISharedViewLoader<TViewInterface> : ISharedViewLoaderBase
	{
		TViewInterface? CachedView { get; }
		UniTask<TViewInterface> Load(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
	}
	
	public interface ISharedViewLoader<TViewInterface, TKey> : ISharedViewLoaderBase
	{
		TViewInterface? CachedView { get; }
		UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
	}
}