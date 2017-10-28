using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Enum;
using ZStart.Core.Manager;
using ZStart.Core.Model;

namespace ZStart.Core.Controller
{
    public enum BundleLoadState
    {
        InProgress,
        Failure,
        Success
    }

    [System.Serializable]
    public class BundleLoadInfo
    {
        public int id;
        public string address = "";
        public BundleType type = BundleType.Unknow;
        public bool isWeb = false;
        public UnityAction<string, bool> completeFun;
        public BundleLoadInfo(string path, BundleType kind)
        {
            address = path;
            type = kind;
        }
    }

    public class ZBundleController : ZSingletonBehaviour<ZBundleController>
    {
        public List<BundleLoadInfo> loadInfoList;
        public BundleLoadState state = BundleLoadState.Failure;
        public int loadIndex = 0;
        public string[] activeVariants = { };
        public int totalLength = 1;

        public float loadingProgress
        {
            get
            {
                if (state == BundleLoadState.Success || state == BundleLoadState.Failure)
                    return 1;
                return loadIndex / (float)totalLength;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            loadInfoList = new List<BundleLoadInfo>();
        }

        public static string GetStreamingAssetsPath()
        {
            if (Application.isEditor)
                return "file://" + System.Environment.CurrentDirectory.Replace("\\", "/"); // Use the build output folder directly.
            else if (Application.platform == RuntimePlatform.WebGLPlayer)
                return System.IO.Path.GetDirectoryName(Application.absoluteURL).Replace("\\", "/") + "/StreamingAssets";
            else if (Application.isMobilePlatform || Application.isConsolePlatform)
                return Application.streamingAssetsPath;
            else // For standalone player.
                return "file://" + Application.streamingAssetsPath;
        }

        public void LoadByURL(string url, BundleType type, UnityAction<string, bool> complete = null)
        {
            if (!HasLoadInfo(url))
            {
                BundleLoadInfo info = new BundleLoadInfo(url, type);
                info.isWeb = true;
                info.completeFun = complete;
                loadInfoList.Add(info);
            }
            totalLength = loadInfoList.Count;
            if (state != BundleLoadState.InProgress)
                LoadNext();
        }

        public void LoadByPath(string path, BundleType type,UnityAction<string,bool> complete = null)
        {
            if (!HasLoadInfo(path))
            {
                BundleLoadInfo info = new BundleLoadInfo(path, type);
                info.isWeb = false;
                info.completeFun = complete;
                loadInfoList.Add(info);
            }
            totalLength = loadInfoList.Count;
            if (state != BundleLoadState.InProgress)
                LoadNext();
        }

        public string RemapVariantName(string assetBundleName)
        {
            string[] bundlesWithVariant = { };

            string[] split = assetBundleName.Split('.');

            int bestFit = int.MaxValue;
            int bestFitIndex = -1;
            // Loop all the assetBundles with variant to find the best fit variant assetBundle.
            for (int i = 0; i < bundlesWithVariant.Length; i++)
            {
                string[] curSplit = bundlesWithVariant[i].Split('.');
                if (curSplit[0] != split[0])
                    continue;

                int found = System.Array.IndexOf(activeVariants, curSplit[1]);

                // If there is no active variant found. We still want to use the first 
                if (found == -1)
                    found = int.MaxValue - 1;

                if (found < bestFit)
                {
                    bestFit = found;
                    bestFitIndex = i;
                }
            }

            if (bestFit == int.MaxValue - 1)
            {
                Debug.LogWarning("Ambigious asset bundle variant chosen because there was no matching active variant: " + bundlesWithVariant[bestFitIndex]);
            }

            if (bestFitIndex != -1)
            {
                return bundlesWithVariant[bestFitIndex];
            }
            else
            {
                return assetBundleName;
            }
        }
       
        public void LoadBundles(BundleInfo[] list)
        {
            if (state == BundleLoadState.InProgress)
                return;
            ZLog.Log("LoadBundles ..................");
            state = BundleLoadState.Failure;
            loadIndex = 0;
            loadInfoList.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                BundleInfo model = list[i];
                if (!HasLoadInfo(model.url))
                {
                    BundleLoadInfo info = new BundleLoadInfo(model.url, model.type);
                    info.id = model.id;
                    info.isWeb = true;
                    loadInfoList.Add(info);
                }
            }
            totalLength = loadInfoList.Count;
            LoadNext();
        }

        void LoadNext()
        {
            if (loadInfoList.Count < 1)
            {
                state = BundleLoadState.Success;
                totalLength = 0;
                loadIndex = 0;
                return;
            }
            BundleLoadInfo info = loadInfoList[0];
            loadIndex++;
            state = BundleLoadState.InProgress;
            if (ZBundleManager.Instance.HasBundle(info.address))
            {
                if (info.completeFun != null)
                    info.completeFun.Invoke(info.address,true);
                RemoveLoadInfo(info.address);
                LoadNext();
            }
            else
            {
                if(info.isWeb)
                    StartCoroutine(LoadWWWInspector(info));
                else
                    StartCoroutine(LoadFileInspector(info));
            }
                
        }

        IEnumerator LoadFileInspector(BundleLoadInfo info)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(info.address);
            yield return request;
           
            if (request.isDone)
            {
                AssetBundle bundle = request.assetBundle;
                if (bundle != null)
                    ZBundleManager.Instance.AddBundle(info.address, info.type, bundle);
                if (info.completeFun != null)
                    info.completeFun.Invoke(info.address, true);
            }
            RemoveLoadInfo(info.address);
            yield return null;
            LoadNext();
        }

        IEnumerator LoadWWWInspector(BundleLoadInfo info)
        {
            while (!Caching.ready)
                yield return null;
            using (WWW www = WWW.LoadFromCacheOrDownload(info.address, 1))
            {
                yield return www;
                if (www.isDone)
                {
                    if (!string.IsNullOrEmpty(www.error))
                    {
                        Debug.LogError("WWW download:" + www.error + " that url = " + info.address);
                        state = BundleLoadState.Failure;
                        if (info.completeFun != null)
                            info.completeFun.Invoke(info.address, false);
                        yield break;
                    }
                    else
                    {
                        ZBundleManager.Instance.AddBundle(info.id, info.address, 1, info.type, www.assetBundle);
                        if (info.completeFun != null)
                            info.completeFun.Invoke(info.address, true);
                    }
                    RemoveLoadInfo(info.address);
                    www.Dispose();
                    LoadNext();
                }
            }
        }

        private void RemoveLoadInfo(string address)
        {
            for (int i = 0; i < loadInfoList.Count; i++)
            {
                BundleLoadInfo mode = loadInfoList[i];
                if (mode.address.Equals(address))
                {
                    loadInfoList.RemoveAt(i);
                    break;
                }
            }
        }

        private BundleLoadInfo GetLoadILnfo(string address)
        {
            foreach (BundleLoadInfo mode in loadInfoList)
            {
                if (mode.address.Equals(address))
                    return mode;
            }
            return null;
        }

        private bool HasLoadInfo(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;
            for (int i = 0; i < loadInfoList.Count; i++)
            {
                BundleLoadInfo mode = loadInfoList[i];
                if (mode.address.Equals(address))
                    return true;
            }
            return false;
        }
    }
}

