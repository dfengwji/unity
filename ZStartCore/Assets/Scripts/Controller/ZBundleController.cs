using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
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
        public uint id;
        public string path = "";
        public string downURL = "";
        public BundleType type = BundleType.Unknow;
        public UnityAction<string, bool> completeFun;
        public BundleLoadInfo(string path, BundleType kind)
        {
            this.path = path;
            type = kind;
        }

        public bool IsWeb
        {
            get
            {
                return string.IsNullOrEmpty(downURL) ? false : true;
            }
        }
    }

    public class ZBundleController : ZSingletonBehaviour<ZBundleController>
    {
        public List<BundleLoadInfo> requestList;
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
            requestList = new List<BundleLoadInfo>();
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

        public void Load(string url,string savePath, BundleType type, UnityAction<string, bool> complete = null)
        {
            if (string.IsNullOrEmpty(savePath))
                return;
            if (!HasLoadInfo(savePath))
            {
                BundleLoadInfo info = new BundleLoadInfo(savePath, type)
                {
                    completeFun = complete,
                    downURL = url
                };
                requestList.Add(info);
            }
            totalLength = requestList.Count;
            if (state != BundleLoadState.InProgress)
                LoadNext();
        }

        public void Load(string path, BundleType type,UnityAction<string,bool> complete = null)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return;
            if (!HasLoadInfo(path))
            {
                BundleLoadInfo info = new BundleLoadInfo(path, type)
                {
                    completeFun = complete
                };
                requestList.Add(info);
            }
            totalLength = requestList.Count;
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
            if (state == BundleLoadState.InProgress || list == null)
                return;
            ZLog.Log("LoadBundles ..................num = " + list.Length);
            state = BundleLoadState.Failure;
            loadIndex = 0;
            requestList.Clear();
            for (int i = 0; i < list.Length; i++)
            {
                BundleInfo model = list[i];
                if (!HasLoadInfo(model.path))
                {
                    BundleLoadInfo info = new BundleLoadInfo(model.path, model.type)
                    {
                        id = model.id,
                        downURL = model.url
                    };
                    requestList.Add(info);
                }
            }
            totalLength = requestList.Count;
            LoadNext();
        }

        void LoadNext()
        {
            if (requestList.Count < 1)
            {
                state = BundleLoadState.Success;
                totalLength = 0;
                loadIndex = 0;
                return;
            }
            BundleLoadInfo info = requestList[0];
            loadIndex++;
            state = BundleLoadState.InProgress;
            if (ZBundleManager.Instance.HasBundle(info.path))
            {
                if (info.completeFun != null)
                    info.completeFun.Invoke(info.path,true);
                RemoveLoadInfo(info.path);
                LoadNext();
            }
            else
            {
                if(info.IsWeb)
                    StartCoroutine(LoadRemoteInspector(info));
                else
                    StartCoroutine(LoadLocalInspector(info));
            }
                
        }

        IEnumerator LoadLocalInspector(BundleLoadInfo info)
        {
            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(info.path);
            yield return request;
           
            if (request.isDone)
            {
                AssetBundle bundle = request.assetBundle;
                if (bundle != null)
                    ZBundleManager.Instance.AddBundle(info.path, info.type, bundle);
                if (info.completeFun != null)
                    info.completeFun.Invoke(info.path, true);
            }
            RemoveLoadInfo(info.path);
            yield return null;
            LoadNext();
        }

        IEnumerator LoadRemoteInspector(BundleLoadInfo info)
        {
            string dir = Path.GetDirectoryName(info.path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(info.path))
                File.Delete(info.path);
            yield return null;
            UnityWebRequest downloader = UnityWebRequest.Get(info.downURL);
            downloader.disposeDownloadHandlerOnDispose = true;
            downloader.downloadHandler = new DownloadHandlerFile(info.path);
            downloader.SendWebRequest();
            while (!downloader.isDone)
            {
                yield return null;
            }
            if (!string.IsNullOrEmpty(downloader.error))
            {
                ZLog.Warning("download asset bundle file failed!!!!" + downloader.error + "; url = " + info.downURL);
                if (info.completeFun != null)
                {
                    info.completeFun.Invoke(info.downURL, false);
                }
                downloader.Dispose();
                downloader = null;
            }
            else
            {
                ZLog.Warning("download asset bundle success that url = " + info.downURL);
                yield return null;
                downloader.Abort();
                downloader.Dispose();
                downloader = null;
                Resources.UnloadUnusedAssets();
                yield return null;
                if (info.completeFun != null)
                {
                    info.completeFun.Invoke(info.downURL, true);
                }
                AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(info.path);
                yield return request;
                if (request.isDone)
                {
                    AssetBundle bundle = request.assetBundle;
                    ZLog.Warning("read asset bundle success that path = " + info.path);
                    if (bundle != null)
                        ZBundleManager.Instance.AddBundle(info.path, info.type, bundle);
                    if (info.completeFun != null)
                        info.completeFun.Invoke(info.path, true);
                }
            }
            RemoveLoadInfo(info.path);
            LoadNext();
        }

        private void RemoveLoadInfo(string address)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                BundleLoadInfo mode = requestList[i];
                if (mode.path == address)
                {
                    requestList.RemoveAt(i);
                    break;
                }
            }
        }

        private BundleLoadInfo GetLoadILnfo(string address)
        {
            foreach (BundleLoadInfo mode in requestList)
            {
                if (mode.path.Equals(address))
                    return mode;
            }
            return null;
        }

        private bool HasLoadInfo(string address)
        {
            if (string.IsNullOrEmpty(address))
                return false;
            for (int i = 0; i < requestList.Count; i++)
            {
                BundleLoadInfo mode = requestList[i];
                if (mode.path == address)
                    return true;
            }
            return false;
        }
    }
}

