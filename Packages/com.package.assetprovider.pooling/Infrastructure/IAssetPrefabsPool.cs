using System.Threading;
using Cysharp.Threading.Tasks;
using Package.Pooling.Infrastructures;
using UnityEngine;

namespace Package.AssetProvider.Pooling.Infrastructure
{
	public interface IAssetPrefabsPool<T> where T : Component
	{
		public UniTask<IPoolReference<T>> GetReference(string key, int size, CancellationToken cancellationToken = default);
	}
}