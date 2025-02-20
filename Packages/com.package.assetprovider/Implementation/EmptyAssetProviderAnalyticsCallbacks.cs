using System;
using Package.AssetProvider.Infrastructure;

namespace Package.AssetProvider.Implementation
{
	public class EmptyAssetProviderAnalyticsCallbacks : IAssetProviderAnalyticsCallbacks
	{
		public void OnRemoteAssetsPreloadStarted(long totalSizeToDownload, string keysFormatted)
		{
		}

		public void OnRemoteAssetsPreloadFailed(Exception error, string keysFormatted)
		{
		}

		public void OnRemoteAssetsPreloadFinished(long totalSizeToDownload, string keysFormatted, TimeSpan duration)
		{
		}
	}
}
