using System;

namespace Package.AssetProvider.Infrastructure
{
	public interface IAssetProviderAnalyticsCallbacks
	{
		void OnRemoteAssetsPreloadStarted(long totalSizeToDownload, string keysFormatted);
		void OnRemoteAssetsPreloadFailed(Exception error, string keysFormatted);
		void OnRemoteAssetsPreloadFinished(long totalSizeToDownload, string keysFormatted, TimeSpan duration);
	}
}
