namespace Common.DeviceInfo
{
	public interface IDeviceInfo
	{
		string GetOsName();
		string GetOsVersion();
		OsVersion GetNumericOsVersion();
		string GetManufacturer();
		string GetDeviceModelName();
		string GetDeviceName();
		string GetPlatformName();
		string GetDeviceId();
		long GetSystemUptime();
		string GetClientVersion();
		int GetBuildNumber();
		long GetSystemMemory();
		bool IsPowerSaveMode();
	}
}
