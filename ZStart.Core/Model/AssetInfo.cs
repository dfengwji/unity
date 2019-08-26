using ZStart.Core.Enum;
namespace ZStart.Core.Model
{
    [System.Serializable]
    public struct AssetInfo
    {
        public string name;
        public AssetType type;
        public string asset;
        public uint audio;
        public uint bundle;
        public int preInstant;
        public int grade;
    }

    [System.Serializable]
    public struct AssetVarietyInfo
    {
        public uint id;
        public string prefabName;
        public uint prefabBundle;
        public string spriteName;
        public uint spriteBundle;
        public SpriteFormatType spriteType;
    }
}
