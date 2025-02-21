#if UNITY_IOS
using System.Runtime.InteropServices;
using DevSettings;
using NSubstitute.Routing.Handlers;
using UnityEngine;
using UnityEngine.iOS;

namespace Common.DeviceInfo
{
	public class IosDeviceInfo : IDeviceInfo
	{
		private OsVersion? _osVersion;

		[DllImport("__Internal")]
		private static extern long GetDeviceUptime();

		public string GetOsName()
		{
			return "iOS";
		}

		public string GetOsVersion()
		{
			return Device.systemVersion;
		}

		public OsVersion GetNumericOsVersion()
		{
			return _osVersion ??= OsVersionParser.Parse(GetOsVersion());
		}

		public string GetManufacturer()
		{
			return "Apple Inc.";
		}

		public string GetDeviceModelName()
		{
			return SystemInfo.deviceModel;
		}

		public string GetDeviceName()
		{
			return SystemInfo.deviceName;
		}

		public string GetPlatformName()
		{
			return Application.platform.ToString();
		}

		public string GetDeviceId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public long GetSystemUptime()
		{
			return GetDeviceUptime();
		}

		public string GetClientVersion()
		{
			return Application.version;
		}
		
		public int GetBuildNumber()
		{
			return new DevSettingsProvider().GetBuildInfo().BuildNumber;
		}
        
		public long GetSystemMemory()
		{
			return SystemInfo.systemMemorySize * 1000000u;
		}

		public bool IsPowerSaveMode()
		{
			return Device.lowPowerModeEnabled;
		}
	}
}
#endif
