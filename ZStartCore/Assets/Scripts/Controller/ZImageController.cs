using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ZStart.Core.Manager;
using ZStart.Core.Util;

namespace ZStart.Core.Controller
{
    public class ZImageController : ZSingletonBehaviour<ZImageController>
    {
        public class ImageInfo
        {
            private string uid = "";
            public string identify = "";
            public Texture2D texture = null;
            public string url = "";
            public Vector2 size = Vector2.zero;
            public byte[] buffer;
            public bool local = false;

            public ImageInfo() { }

            public ImageInfo(string uuid, string app, Texture2D icon, string url)
            {
                this.uid = uuid;
                this.identify = app;
                this.texture = icon;
                this.url = url;
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
            public bool isCache;
            public bool readable;
            public string path;
            public UnityAction<ImageInfo> callFun;

            public override string ToString()
            {
                return "uid = " + uid + ";url = " + url + ";identify = " + identify;
            }
        }
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

        private string GetUID(string identify, string url)
        {
            return identify + "_" + MD5Util.MD5String(url);
        }

        /// <summary>
        /// 默认获取logo
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="package"></param>
        /// <param name="url"></param>
        /// <param name="flag">logo == -1</param>
        private void LoadRemote(string identify, string url, bool cache, bool read, string path, UnityAction<ImageInfo> fun)
        {
            //ZLog.Log("ImageLoadController.....uid = " + appid + "; package = " + package + "; url = " + url + "; index = " + flag + ";path = " + path);
            if (string.IsNullOrEmpty(url))
            {
                LoadFromApk(identify);
                return;
            }
            string uid = GetUID(identify, url);
            if (HasRequest(uid))
                return;
            RequestInfo info = new RequestInfo
            {
                uid = uid,
                identify = identify,
                url = url,
                package = identify,
                callFun = fun,
                isCache = cache,
                readable = read,
                path = path
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
                uid = GetUID(identify, path),
                identify = identify,
                package = "",
                url = path,
                isCache = true,
                readable = true
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

        public void Load(string identify, string url, Vector2 size, UnityAction<ImageInfo> call, bool cache = true, bool read = true, string path = "")
        {
            string uid = GetUID(identify, url);
            ImageInfo info = GetImageInfo(uid);
            if (info != null)
            {
                if(call != null)
                {
                    call.Invoke(info);
                }
                return;
            }
            if (IsWebPath(url))
            {
                LoadRemote(identify, url, cache, read, path, call);
            }
            else
            {
                if (!File.Exists(url))
                {
                    Debug.LogError("the file not exist by the path that = " + url);
                    return;
                }
                ImageInfo tmp = ReadLocal(identify, url, size);
                if (tmp != null && call != null)
                    call.Invoke(tmp);
            }
        }

        public void Load(string identify, string url, UnityAction<ImageInfo> call, bool cache = true)
        {
            Load(identify, url, defaultSize, call, cache);
        }

        public ImageInfo GetImageInfo(string identify, string url, UnityAction<ImageInfo> call, bool cache = true, bool read = true)
        {
            string uid = GetUID(identify, url);
            ImageInfo info = GetImageInfo(uid);
            if (info != null && info.url == url)
                return info;
            if (IsWebPath(url))
            {
                LoadRemote(identify, url, cache, read, "", call);
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
                    if (File.Exists(info.url))
                    {
                        byte[] bytes = File.ReadAllBytes(info.url);
                        AddImage(bytes, info, "", new Vector2(defaultSize.x, defaultSize.y));
                        RemoveRequest(info.uid);
                        LoadNext();
                    }
                    else
                    {
                        StartCoroutine(DownloadInspector(info));
                    }
                }
                else
                {
                    StartCoroutine(DownloadInspector(info));
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
           
        }

        private Texture2D CreateTexture(byte[] bytes, Vector2 size)
        {
            //ZLog.Warning("create texture start...."+Time.realtimeSinceStartup);
            Texture2D texture = new Texture2D((int)size.x, (int)size.y, TextureFormat.RGBA32, false);
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.LoadImage(bytes);
            //ZLog.Warning("create texture end...." + Time.realtimeSinceStartup);
            return texture;
        }

        private IEnumerator LoadFromDisk(RequestInfo info)
        {
            isLoading = true;
            using (UnityWebRequest loader = UnityWebRequestTexture.GetTexture("file://" + info.url))
            {
                loader.downloadHandler = new DownloadHandlerTexture(info.readable);
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

        private IEnumerator DownloadInspector(RequestInfo info)
        {
            isLoading = true;
            UnityWebRequest loader = null;
            //ZLog.Warning("LoadInspector...." + info.url + "---"+info.screenIdx);
            using (loader = UnityWebRequestTexture.GetTexture(info.url))
            {
                DownloadHandlerTexture texHandler = new DownloadHandlerTexture(info.readable);
                loader.disposeDownloadHandlerOnDispose = true;
                loader.downloadHandler = texHandler;
                yield return loader.SendWebRequest();

                if (!string.IsNullOrEmpty(loader.error))
                {
                    LoadFromApk(info.package);
                    ZLog.Warning("Image load error ....." + loader.error);
                }
                else
                {

                    //Texture2D tex = DownloadHandlerTexture.GetContent(loader);
                    AddImage(texHandler.data, info, info.path, new Vector2(texHandler.texture.width, texHandler.texture.height));
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
                    ZFileManager.Instance.WriteFile(path, buffer);
                }
                Texture2D texture = CreateTexture(buffer, new Vector2(size.x, size.y));
                ImageInfo detail = new ImageInfo(info.uid, info.identify, texture, info.url)
                {
                    local = false
                };
                loadedList.Add(detail);
                if(info.callFun != null)
                {
                    info.callFun.Invoke(detail);
                }
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
