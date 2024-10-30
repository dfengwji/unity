using UnityEngine;
using ZStart.Core.Enum;
namespace ZStart.Core.Model
{
    public class BundleModel:ZDataBase
    {
        public AssetBundle bundle = null;
        public uint version = 0;
        public string address = "";
        public int count = 0;
        public BundleType type = BundleType.Texture2D;
        public BundleModel()
        {

        }

        public BundleModel(string path, uint ver, BundleType kind)
        {
            address = path;
            version = ver;
            type = kind;
            name = path;
            UID = address + "_" + ver;
        }

        public override void Dispose()
        {
            if (bundle != null)
                bundle.Unload(true);
            bundle = null;
            version = 0;
            address = "";
            count = 0;
            ID = 0;
            name = "";
            UID = "";
            type = BundleType.Unknow;
        }
    }

    public struct BundleInfo
    {
        public BundleType type;
        public string url;
        public string path;
        public uint version;
        public uint id;
        public int size;
        public string name;
        public uint crc;
        public string md5;
    }
}
