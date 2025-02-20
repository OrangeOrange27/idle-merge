namespace Common.AssetProvider.Pooling.Tests.Data
{
	public class TestAssetsProviderInstanceCounter
	{
		public int NotDisposedCount { get; private set; }
		public int TotalCreatedSoFarCount { get; private set; }

		public int ViewPrefabsLoadedCount { get; private set;  }

		public int AssetProviderCreated()
		{
			NotDisposedCount++;
			TotalCreatedSoFarCount++;

			return TotalCreatedSoFarCount;
		}

		public void AssetProviderDisposed()
		{
			NotDisposedCount--;
		}

		public void ViewInstanceCreated()
		{
			ViewPrefabsLoadedCount++;
		}

		public void ViewInstanceDestroyed()
		{
			ViewPrefabsLoadedCount--;
		}
	}
}