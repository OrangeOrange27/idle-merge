using DevSettings.Interfaces;
using UnityEngine;

namespace DevSettings
{
	public class DevSettingsProvider : IDevSettingsProvider
	{
		public const string devSettingsAssetKey = "DevSettingsAsset";

		private readonly DevSettingsScriptable _asset;

		public DevSettingsProvider()
		{
			_asset = Resources.Load<DevSettingsScriptable>(devSettingsAssetKey);
		}

		public IBuildInfo GetBuildInfo()
		{
			return _asset.buildInfo;
		}
	}
}
