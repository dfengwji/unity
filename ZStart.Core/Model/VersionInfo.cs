namespace ZStart.Core.Model
{
    public struct VersionInfo
    {
        public string version;
        public uint revision;
        public uint minor;
        public uint major;
        public uint build;

        public bool isCode
        {
            get
            {
                return build > 0 ? true : false;
            }
        }
    }
}
