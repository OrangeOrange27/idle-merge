using System;
using DevSettings;
using UnityEngine;

namespace Common.DeviceInfo
{
	public class AndroidDeviceInfo : IDeviceInfo
	{
		private const string ElapsedRealtimeMethodName = "elapsedRealtime";

		private readonly AndroidJavaObject _systemClockObject;

		private string _versionCode;
		private OsVersion? _osVersion;

		public AndroidDeviceInfo()
		{
			_systemClockObject = new AndroidJavaObject("android.os.SystemClock");
		}

		public string GetOsName()
		{
			return "Android";
		}

		public string GetOsVersion()
		{
			using (var buildVersion = new AndroidJavaClass("android/os/Build$VERSION"))
			{
				return buildVersion.GetStatic<string>("RELEASE");
			}
		}

		public OsVersion GetNumericOsVersion()
		{
			return _osVersion ??= OsVersionParser.Parse(GetOsVersion());
		}

		public string GetManufacturer()
		{
			string manufacturer;
			using (var buildClass = new AndroidJavaClass("android/os/Build"))
			{
				manufacturer = buildClass.GetStatic<string>("MANUFACTURER");
			}

			return manufacturer;
		}

		public string GetDeviceModelName()
		{
			string model;
			string manufacturer;
			using (var buildClass = new AndroidJavaClass("android/os/Build"))
			{
				model = buildClass.GetStatic<string>("MODEL");
				manufacturer = buildClass.GetStatic<string>("MANUFACTURER");
			}

			if (model.StartsWith(manufacturer, StringComparison.OrdinalIgnoreCase))
			{
				model = model.Substring(manufacturer.Length + 1);
			}

			return model;
		}

		public string GetDeviceName()
		{
			using var contentResolver = GetContentResolver();
			using var secureSettings = new AndroidJavaClass("android/provider/Settings$Secure");
			return secureSettings.CallStatic<string>("getString", contentResolver, "device_name");
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
			return _systemClockObject.CallStatic<long>(ElapsedRealtimeMethodName);
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
			long memorySize;

			using (var currentActivity = GetCurrentActivity())
			{
				using (var systemService = currentActivity.Call<AndroidJavaObject>("getSystemService", "activity"))
				{
					using (var memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo"))
					{
						systemService.Call("getMemoryInfo", memoryInfo);

						using (var memInfo = memoryInfo)
						{
							memorySize = memInfo.Get<long>("totalMem");
						}
					}
				}
			}

			return memorySize;
		}

		public bool IsPowerSaveMode()
		{
			bool isPowerSaveMode;

			try
			{
				using var currentActivity = GetCurrentActivity();
				using var powerManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "power");

				isPowerSaveMode = powerManager.Call<bool>("isPowerSaveMode");
			}
			catch
			{
				isPowerSaveMode = false;
			}

			return isPowerSaveMode;
		}

		private static AndroidJavaObject GetContentResolver()
		{
			using (var currentActivity = GetCurrentActivity())
			{
				return currentActivity.Call<AndroidJavaObject>("getContentResolver");
			}
		}

		private static AndroidJavaObject GetCurrentActivity()
		{
			using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				return unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
			}
		}
	}
}
