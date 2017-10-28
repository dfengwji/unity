using ZStart.Core.Enum;
namespace ZStart.Core.Model
{
    [System.Serializable]
    public struct AssetInfo
    {
        public string name;
        public AssetType type;
        public string asset;
        public int audio;
        public int bundle;
        public int preInstant;
        public int grade;
    }

    [System.Serializable]
    public struct AssetVarietyInfo
    {
        public int id;
        public string prefabName;
        public int prefabBundle;
        public string spriteName;
        public int spriteBundle;
        public SpriteFormatType spriteType;
    }
}
