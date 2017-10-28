using ZStart.Core.Enum;
namespace ZStart.Core.Model
{
    [System.Serializable]
    public struct EffectInfo
    {
        public int id;
        public string name;
        public string assetPath;
        public float lifeTime;
        public bool isLoop;
        public EffectType type;
        public int bundle;
    }
}
