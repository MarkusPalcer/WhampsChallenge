namespace ContractGenerator
{
    public struct VersionInfo
    {
        public int Major;
        public int Minor;
        public int Revision;

        public VersionInfo(int major, int minor, int revision)
        {
            Major = major;
            Minor = minor;
            Revision = revision;
        }
    }
}