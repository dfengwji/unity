using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using ZStart.Common.Enum;
using ZStart.Common.Manager;
using ZStart.Core;

namespace ZStart.Common.Controller
{
    public class ImageInfo
    {
        private string uid = "";
        public string identify = "";
        public Texture2D texture = null;
        public string url = "";
        public int flag = 0;
        public Vector2 size = Vector2.zero;
        public byte[] buffer;
        public bool local = false;

        public ImageInfo() { }

        public ImageInfo(string uuid, string app, Texture2D icon, string url, int flag)
        {
            this.uid = uuid;
            this.identify = app;
            this.texture = icon;
            this.url = url;
            this.flag = flag;
        }

        public string GetUID()
        {
            return uid;
        }

        public void Clear()
        {
            texture = null;
            buffer = null;
        }
    }

    public struct RequestInfo
    {
        public string uid;
        public string identify;
        public string package;
        public string url;
        public int flag;
        public bool isCache;

        public override string ToString()
        {
            return "uid = " + uid + ";url = " + url + ";identify = " + identify;
        }
    }

    public class ImageLoadController : ZSingletonBehaviour<ImageLoadController>
    {
        private List<RequestInfo> requestList;

        public List<ImageInfo> loadedList;
        public Vector2 defaultSize = new Vector2(400, 225);
        private bool isLoading;

        protected override void Awake()
        {
            base.Awake();
            isLoading = false;
            requestList = new List<RequestInfo>();
            loadedList = new List<ImageInfo>();
        }

        private string GetUID(string identify, int index)
        {
            return identify + "_" + index;
        }

        /// <summary>
        /// 默认获取logo
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="package"></param>
        /// <param name="url"></param>
        /// <param name="flag">logo == -1</param>
        private void LoadRemote(string identify, string url, int flag, bool cache)
        {
            //ZLog.Log("ImageLoadController.....uid = " + appid + "; package = " + package + "; url = " + url + "; index = " + flag + ";path = " + path);
            if (string.IsNullOrEmpty(url))
            {
                LoadFromApk(identify);
                return;
            }
            string uid = GetUID(identify, flag);
            if (HasRequest(uid))
                return;
            RequestInfo info = new RequestInfo
            {
                uid = uid,
                identify = identify,
                url = url,
                package = identify,
                flag = flag,
                isCache = cache
            };
            requestList.Add(info);

            LoadNext();
        }

        private ImageInfo ReadLocal(string identify, string path, Vector2 size)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;
            byte[] bytes = File.ReadAllBytes(path);
            RequestInfo info = new RequestInfo()
            {
                uid = GetUID(identify, -1),
                identify = identify,
                flag = -1,
                package = "",
                url = path,
                isCache = true
            };
            ImageInfo tmp = AddImage(bytes, info, "", new Vector2(size.x, size.y));
            if (tmp != null)
            {
                return tmp;
            }
            return null;
        }

        private bool IsWebPath(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
            int len = 6;
            if (url.Length < len)
                len = url.Length;
            string sub = url.Substring(0, len);
            if (sub.Contains("http") || sub.Contains("https") || sub.Contains("ftp"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public Texture2D GetTexture(string identify, string url, Vector2 size, int flag = 0, bool cache = true)
        {
            string uid = GetUID(identify, flag);
            ImageInfo info = GetImageInfo(uid);
            if (info != null)
                return info.texture;
            if (IsWebPath(url))
            {
                LoadRemote(identify, url, flag, cache);
            }
            else
            {
                ImageInfo tmp = ReadLocal(identify, url, size);
                if (tmp != null)
                    return tmp.texture;
            }
            return null;
        }

        public Texture2D GetTexture(string identify, string url, int flag = 0, bool cache = true)
        {
            return GetTexture(identify, url, defaultSize, flag, cache);
        }

        public ImageInfo GetImageInfo(string identify, string url, int flag = 0, bool cache = true)
        {
            string uid = GetUID(identify, flag);
            ImageInfo info = GetImageInfo(uid);
            if (info != null && info.url == url)
                return info;
            if (IsWebPath(url))
            {
                LoadRemote(identify, url, flag, cache);
            }
            else
            {
                return ReadLocal(identify, url, defaultSize);
            }
            return null;
        }

        private ImageInfo GetImageInfo(string uid)
        {
            for (int i = 0; i < loadedList.Count; i++)
            {
                if (loadedList[i].GetUID() == uid)
                    return loadedList[i];
            }
            return null;
        }

        private bool HasRequest(string uid)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].uid == uid)
                    return true;
            }
            return false;
        }

        private void LoadNext()
        {
            if (isLoading)
                return;
            if (requestList.Count > 0)
            {
                RequestInfo info = requestList[0];
                if (info.isCache)
                {
                    string path = Path.Combine(FileManager.LogoDirectory, info.identify + ".jpg");
                    if (File.Exists(path))
                    {
                        byte[] bytes = File.ReadAllBytes(path);
                        AddImage(bytes, info, "", new Vector2(defaultSize.x, defaultSize.y));
                        RemoveRequest(info.uid);
                        LoadNext();
                    }
                    else if (File.Exists(info.url))
                    {
                        byte[] bytes = File.ReadAllBytes(info.url);
                        AddImage(bytes, info, "", new Vector2(defaultSize.x, defaultSize.y));
                        RemoveRequest(info.uid);
                        LoadNext();
                    }
                    else
                    {
                        StartCoroutine(DownloadInspector(info, path));
                    }
                }
                else
                {
                    StartCoroutine(DownloadInspector(info, null));
                }
            }
        }

        private void RemoveRequest(string uid)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].uid == uid)
                {
                    requestList.RemoveAt(i);
                    return;
                }
            }
        }

        private void LoadFromApk(string package)
        {
            NotifyManager.SendNotify(NotifyType.OnAppIconLoad, null);
        }

        private Texture2D CreateTexture(byte[] bytes, Vector2 size)
        {
            //ZLog.Warning("create texture start...."+Time.realtimeSinceStartup);
            Texture2D texture = new Texture2D((int)size.x, (int)size.y);
            texture.LoadImage(bytes);
            //ZLog.Warning("create texture end...." + Time.realtimeSinceStartup);
            return texture;
        }

        private IEnumerator LoadFromDisk(RequestInfo info)
        {
            isLoading = true;
            using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + info.url))
            {
                loader.downloadHandler = new DownloadHandlerTexture(true);
                yield return loader.SendWebRequest();

                if (!string.IsNullOrEmpty(loader.error))
                {
                    ZLog.Warning("Image load error ....." + loader.error + ";url = " + info.url);
                }
                else
                {
                    Texture2D tex = DownloadHandlerTexture.GetContent(loader);
                    AddImage(tex.EncodeToJPG(), info, "", new Vector2(tex.width, tex.height));
                }
                yield return null;
                loader.Dispose();
            }
            RemoveRequest(info.uid);
            isLoading = false;

            yield return null;
            LoadNext();
        }

        private IEnumerator DownloadInspector(RequestInfo info, string path)
        {
            isLoading = true;
            UnityWebRequest loader = null;
            //ZLog.Warning("LoadInspector...." + info.url + "---"+info.screenIdx);

            using (loader = UnityWebRequestTexture.GetTexture(info.url))
            {
                DownloadHandlerTexture texHandler = new DownloadHandlerTexture(true);
                loader.disposeDownloadHandlerOnDispose = true;
                loader.downloadHandler = texHandler;
                yield return loader.SendWebRequest();

                if (!string.IsNullOrEmpty(loader.error) || loader.isNetworkError || loader.isHttpError)
                {
                    LoadFromApk(info.package);
                    ZLog.Warning("Image load error ....." + loader.error);
                }
                else
                {

                    //Texture2D tex = DownloadHandlerTexture.GetContent(loader);
                    AddImage(texHandler.data, info, path, new Vector2(texHandler.texture.width, texHandler.texture.height));
                }
            }
            RemoveRequest(info.uid);
            yield return null;
            isLoading = false;
            loader.Dispose();

            yield return null;
            LoadNext();
        }

        private ImageInfo AddImage(byte[] buffer, RequestInfo info, string path, Vector2 size)
        {
            if (buffer != null && size.x > 10 && size.y > 10)
            {
                //ZLog.Warning("generate texture bytes start...." + Time.realtimeSinceStartup);
                //byte[] buffer = tex.EncodeToPNG();
                //ZLog.Warning("generate texture bytes end...." + Time.realtimeSinceStartup);
                if (!string.IsNullOrEmpty(path))
                {
                    FileManager.Instance.WriteFile(path, buffer);
                }
                Texture2D texture = CreateTexture(buffer, new Vector2(size.x, size.y));
                ImageInfo detail = new ImageInfo(info.uid, info.identify, texture, info.url, info.flag)
                {
                    local = false
                };
                loadedList.Add(detail);

                NotifyManager.SendNotify(NotifyType.OnImageUpdate, detail);
                return detail;
            }
            else
            {
                ZLog.Warning("texture is null...");
                return null;
            }
        }

        private void Reset()
        {
            //Resources.UnloadUnusedAssets();
        }
    }
}
