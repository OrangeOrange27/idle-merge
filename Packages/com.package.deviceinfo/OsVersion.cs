namespace Common.DeviceInfo
{
    public readonly struct OsVersion
    {
        public readonly int Major;
        public readonly int Minor;
        public readonly int Revision;

        public OsVersion(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }
        
        public static bool operator ==(OsVersion version1, OsVersion version2)
        {
            return version1.Major == version2.Major && version1.Minor == version2.Minor && version1.Revision == version2.Revision;
        }

        public static bool operator !=(OsVersion version1, OsVersion version2)
        {
            return !(version1 == version2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is OsVersion other))
                return false;

            return this == other;
        }

        public override int GetHashCode()
        {
            return (Major, Minor, Revision).GetHashCode();
        }
    }
}