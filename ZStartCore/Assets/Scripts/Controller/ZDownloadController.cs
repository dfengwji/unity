using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using ZStart.Core.Util;

namespace ZStart.Core.Controller
{
    public class ZDownloadController : ZSingletonBehaviour<ZDownloadController>
    {
        public enum Status
        {
            /// <summary>
            /// 
            /// </summary>
            Pending = 1,
            Running = 2,
            Pause = 4,
            Success = 8,
            Failed = 16,
            Connecting = 32,
            Queue = 64,
        }

        public class RequestInfo
        {
            public string uid = "";
            public string identify = "";
            public string name = "";
            public string url = "";
            public string path = "";
            public long size = 0;
            public uint tryNum = 0;
            public string md5 = "";
            public UnityAction<Status, string, string> callback;
            public Status status = Status.Pending;
        }

        private List<RequestInfo> requests = new List<RequestInfo>();
        private bool isLoading = false;

        public uint maxTry = 3;
        public int total = 0;
        public int current = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        public float AllProgress
        {
            get
            {
                if (total < 1)
                    return 0;
                return current / (float)total;
            }
        }

        public float SingleProgress
        {
            get; set;
        }

        public bool IsComplete
        {
            get
            {
                for (int i = 0; i < requests.Count; i++)
                {
                    if (requests[i].status == Status.Pending || requests[i].status == Status.Running)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        private string GetUID(string identify, long size)
        {
            return identify + "-" + size;
        }

        public void Ready(string identify, string url, string path, long size, string name,UnityAction<Status, string, string> callfun, string md5 = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                ZLog.Warning("download url is empty!!!!that uid = " + identify);
                return;
            }
            string uid = GetUID(identify, size);
            RequestInfo info = GetRequest(uid);
            if (info == null)
            {
                RequestInfo request = new RequestInfo()
                {
                    uid = uid,
                    name = name,
                    identify = identify,
                    url = url,
                    status = Status.Pending,
                    size = size,
                    path = path,
                    md5 = md5,
                    callback = callfun
                };
                requests.Add(request);
                total += 1;
            }
        }

        public void Begin(string identify, string url, string path, long size, string name, UnityAction<Status, string, string> callfun, string md5 = "")
        {
            Ready(identify, url, path, size, name, callfun, md5);
            current = 0;
            total = requests.Count;
            LoadNext();
        }

        public void Begin()
        {
            current = 0;
            total = requests.Count;
            LoadNext();
        }

        public void Begin(RequestInfo[] list)
        {
            total = list.Length;
            current = 0;
            requests.Clear();
            requests.AddRange(list);
            LoadNext();
        }

        public void Pause(string uid)
        {
            RequestInfo info = GetRequest(uid);
            if (info != null)
            {
                info.status = Status.Pause;
            }
        }

        public void Remove(string uid)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].uid == uid && requests[i].status == Status.Queue)
                {
                    requests.RemoveAt(i);
                    break;
                }
            }
        }

        public void Resume(string uid)
        {
            RequestInfo info = GetRequest(uid);
            if (info != null)
            {
                info.status = Status.Pending;
            }
            LoadNext();
        }

        private int GetRequestCount(Status status)
        {
            int num = 0;
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].status == status)
                {
                    num += 1;
                }
            }
            return num;
        }

        private RequestInfo GetNextRequest()
        {
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].status == Status.Pending)
                {
                    return requests[i];
                }
            }
            return null;
        }

        private RequestInfo GetRequest(string uid)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].uid == uid)
                {
                    return requests[i];
                }
            }
            return null;
        }

        private void RemoveRequest(string uid)
        {
            for (int i = 0; i < requests.Count; i++)
            {
                if (requests[i].uid == uid)
                {
                    requests.RemoveAt(i);
                    break;
                }
            }
        }

        private void LoadNext()
        {
            if (isLoading)
                return;
            RequestInfo info = GetNextRequest();
            if (info == null)
            {
                total = 0;
                current = 0;
                return;
            }
            current += 1;
            if (string.IsNullOrEmpty(info.path))
            {
                if (info.callback != null)
                {
                    info.callback.Invoke(Status.Failed, info.identify, info.name);
                }
                RemoveRequest(info.uid);
                LoadNext();
                return;
            }
            StartCoroutine(LoadingInspector(info));
        }

        IEnumerator LoadingInspector(RequestInfo info)
        {
            ZLog.Warning("try to download file from url = " + info.url + " ; and path = " + info.path);
            isLoading = true;
            info.status = Status.Running;
            string dir = Path.GetDirectoryName(info.path);
            if (Directory.Exists(dir) == false)
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(info.path))
                File.Delete(info.path);
            yield return null;
            using (UnityWebRequest downloader = UnityWebRequest.Get(info.url)) { 
            if (info.callback != null)
            {
                info.callback.Invoke(Status.Running, info.identify, info.name);
            }
            downloader.disposeDownloadHandlerOnDispose = true;
            downloader.downloadHandler = new DownloadHandlerFile(info.path);
            downloader.SendWebRequest();
            while (!downloader.isDone)
            {
                SingleProgress = downloader.downloadProgress;
                yield return null;
            }

                if (!string.IsNullOrEmpty(downloader.error))
                {
                    ZLog.Warning("download file failed!!!!" + downloader.error);
                    info.status = Status.Failed;
                    if (info.callback != null)
                    {
                        info.callback.Invoke(Status.Failed, info.identify, info.name);
                    }
                    info.tryNum += 1;
                    downloader.Dispose();
                    yield return null;

                    Resources.UnloadUnusedAssets();
                    if (info.tryNum > maxTry)
                    {
                        RemoveRequest(info.uid);
                        isLoading = false;

                        LoadNext();
                    }
                    else
                    {
                        StartCoroutine(CheckNextDelay(info));
                    }
                }
                else
                {
                    // Or retrieve results as binary data
                    yield return null;

                    if (!string.IsNullOrEmpty(info.md5))
                    {
                        string md5 = MD5Util.MDFile(info.path);
                        bool success = md5 == info.md5 ? true : false;
                        info.status = success ? Status.Success : Status.Failed;
                        ZLog.Warning("download file success!!!!check md5 success = " + success + " ;path = " + info.path);
                    }
                    else
                    {
                        info.status = Status.Success;
                        ZLog.Warning("download file success!!!!" + dir + "---" + info.path);
                    }
                    yield return null;
                    if (info.callback != null)
                    {
                        info.callback.Invoke(info.status, info.identify, info.path);
                    }
                    RemoveRequest(info.uid);

                    yield return null;

                    downloader.Abort();
                    downloader.Dispose();
                }
                yield return null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
                yield return null;

                Resources.UnloadUnusedAssets();
                yield return null;
                isLoading = false;
                LoadNext();
            }
        }

        IEnumerator CheckNextDelay(RequestInfo info)
        {
            GC.Collect();
            yield return new WaitForSeconds(2);
            StartCoroutine(LoadingInspector(info));
        }
    }
}
