using ZStart.Common.Enum;

namespace ZStart.Common.Model
{
    public struct ConfigFileInfo
    {
        public long parent;
        public ConfigFileType type;
        public string name;
        public string text;
        public string fullPath;
    }
}
