using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Package.AssetProvider.ViewLoader.Infrastructure
{
	public interface IViewLoader<TViewInterface> : IViewLoaderBase
	{
		UniTask<TViewInterface> Load(ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
	}
	
	public interface IViewLoader<TViewInterface, in TKey> 
	{
		UniTask Prewarm(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, int size = 1);

		UniTask<TViewInterface> Load(TKey key, ICollection<IDisposable> resources, CancellationToken cancellationToken, Transform parent);
	}
}
