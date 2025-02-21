using System.Linq;

namespace Common.DeviceInfo
{
    public static class OsVersionParser
    {
        public static OsVersion Parse(string version)
        {
            if (string.IsNullOrEmpty(version)) return new OsVersion(0, 0, 0);

            var split = version.Split('.');
            var major = int.TryParse(split.ElementAtOrDefault(0), out var majorResult) ? majorResult : 0;
            var minor = int.TryParse(split.ElementAtOrDefault(1), out var minorResult) ? minorResult : 0;
            var revision = int.TryParse(split.ElementAtOrDefault(2), out var revisionResult) ? revisionResult : 0;

            return new OsVersion(major, minor, revision);
        }
    }
}