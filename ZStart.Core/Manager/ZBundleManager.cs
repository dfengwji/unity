using System.Collections.Generic;
using UnityEngine;
using ZStart.Core.Enum;
using ZStart.Core.Model;

namespace ZStart.Core.Manager
{
    public class ZBundleManager
    {
        private static ZBundleManager _instance = null;
        private List<BundleModel> bundleList;
        static string[] m_ActiveVariants = { };
        private ZBundleManager()
        {
            bundleList = new List<BundleModel>();
        }

        public static ZBundleManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ZBundleManager();
                return _instance;
            }
        }

        public void AddBundle(int id,string url, uint version, BundleType type, AssetBundle bundle)
        {
            if (HasBundle(id))
            {
                ZLog.Warning("add bundle failed!!!that had same bundle id = " + id+";url = "+url);
                return;
            }
            BundleModel model = new BundleModel(url, version, type);
            model.bundle = bundle;
            model.ID = id;
            bundleList.Add(model);
        }

        public void AddBundle(string uid,BundleType type,AssetBundle bundle)
        {
            if (HasBundle(uid))
            {
                ZLog.Warning("add bundle failed!!!that had same bundle uid = "+uid);
                return;
            }
            BundleModel model = new BundleModel();
            model.UID = uid;
            model.type = type;
            model.bundle = bundle;
            bundleList.Add(model);
        }

        public bool HasBundle(string uid)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                if (bundleList[i].UID == uid)
                    return true;
            }
            return false;
        }

        public bool HasBundle(int id)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                if (bundleList[i].ID == id)
                    return true;
            }
            return false;
        }

        public bool HasBundle(string url, uint version)
        {
            string keyName = url + "_" + version;
            for (int i = 0; i < bundleList.Count; i++)
            {
                if (bundleList[i].UID == keyName)
                    return true;
            }
            return false;
        }

        public AssetBundle[] GetBundles(BundleType type)
        {
            List<AssetBundle> bundles = new List<AssetBundle>();
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.type == type)
                    bundles.Add(info.bundle);
            }
            return bundles.ToArray();
        }

        public AssetBundle GetBundle(string url, int version)
        {
            string keyName = url + "_" + version;
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == keyName)
                    return info.bundle;
            }
            return null;
        }

        public AssetBundle GetBundle(int id)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.ID == id)
                    return info.bundle;
            }
            return null;
        }

        public AssetBundle GetBundle(string uid)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == uid)
                    return info.bundle;
            }
            return null;
        }

        public AssetBundleRequest GetAssetAsync<T>(string uid,string assetName) where T:Object
        {
            for (int i = 0,max = bundleList.Count; i < max; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == uid)
                {
                    return info.bundle.LoadAssetAsync<T>(assetName);
                }
            }
            return null;
        }

        public T GetAsset<T>(string uid, string assetName) where T : Object
        {
            for (int i = 0, max = bundleList.Count; i < max; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == uid)
                {
                    return info.bundle.LoadAsset<T>(assetName);
                }
            }
            return null;
        }

        public void RemoveBundles(BundleType type)
        {
            List<BundleModel> list = new List<BundleModel>();
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.type == type)
                {
                    if(info.bundle != null)
                        info.bundle.Unload(true);
                    info.Dispose();
                    list.Add(info);
                }
            }
            for (int i = 0; i < list.Count; i++)
            {
                bundleList.Remove(list[i]);
            }
        }

        public void RemoveBundle(string uid)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == uid)
                {
                    if (info.bundle != null)
                        info.bundle.Unload(true);
                    info.Dispose();
                    bundleList.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveBundle(int id)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.ID == id)
                {
                    if (info.bundle != null)
                        info.bundle.Unload(true);
                    info.Dispose();
                    bundleList.RemoveAt(i);
                    break;
                }
            }
        }

        public void UnloadBundle(string uid,bool allObj)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.UID == uid)
                {
                    if (info.bundle != null)
                        info.bundle.Unload(allObj);
                    break;
                }
            }
        }

        public void UnloadBundle(int id,bool allObj)
        {
            for (int i = 0; i < bundleList.Count; i++)
            {
                BundleModel info = bundleList[i];
                if (info.ID == id)
                {
                    if(info.bundle != null)
                        info.bundle.Unload(allObj);
                    break;
                }
            }
        }

        public void UnloadBundle(string url, int version, bool allObj)
        {
            string keyName = url + "_" + version;
            UnloadBundle(keyName,allObj);
        }
    }
}
