using System.Diagnostics;
using DevSettings;
using UnityEngine;

namespace Common.DeviceInfo
{
	public class StandaloneDeviceInfo : IDeviceInfo
	{
		private OsVersion? _osVersion;
		
		public string GetOsName()
		{
			return SystemInfo.operatingSystem;
		}

		public string GetOsVersion()
		{
			return SystemInfo.operatingSystem;
		}
		
		public OsVersion GetNumericOsVersion()
		{
			return _osVersion ??= OsVersionParser.Parse(GetOsVersion());
		}

		public string GetManufacturer()
		{
			return SystemInfo.deviceName;
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

		public string GetStoreName()
		{
			throw new System.NotImplementedException();
		}

		public string GetDeviceModel()
		{
			return string.Empty;
		}
		
		public string GetDeviceId()
		{
			return SystemInfo.deviceUniqueIdentifier;
		}

		public long GetSystemUptime()
		{
			var ticks = Stopwatch.GetTimestamp();
			var uptime = (double)ticks / Stopwatch.Frequency * 1000;

			return (long)uptime;
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
			return false;
		}
	}
}