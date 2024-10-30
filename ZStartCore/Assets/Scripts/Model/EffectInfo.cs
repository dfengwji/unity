using ZStart.Core.Enum;
namespace ZStart.Core.Model
{
    [System.Serializable]
    public struct EffectInfo
    {
        public uint id;
        public string name;
        public string assetPath;
        public float lifeTime;
        public bool isLoop;
        public EffectType type;
        public uint bundle;
    }
}
