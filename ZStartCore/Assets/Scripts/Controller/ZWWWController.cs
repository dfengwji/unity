using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ZStart.Core.Enum;
using ZStart.Core.Model;

namespace ZStart.Core.Controller
{
    public class HttpInfo
    {
        public string uid;
        public int funID;
        public int code;
        public string url;
        public WWWForm form;
        public byte[] bytes;
        public string data;
        public string param;

        public HttpInfo(string id, int fun,int type, string uri,string arg)
        {
            uid = id;
            funID = fun;
            code = type;
            url = uri;
            form = null;
            bytes = null;
            data = "";
            param = arg;
        }

        public HttpInfo(string id, int fun, int type, string uri, WWWForm f,string arg)
        {
            uid = id;
            funID = fun;
            code = type;
            url = uri;
            form = f;
            bytes = null;
            data = "";
            param = arg;
        }

        public HttpInfo(string id, int fun, int type, string uri, byte[] array,string arg)
        {
            uid = id;
            funID = fun;
            url = uri;
            code = type;
            form = null;
            bytes = array;
            data = "";
            param = arg;
        }

        public HttpInfo(string id, int fun, int type, string uri, string json,string arg)
        {
            uid = id;
            funID = fun;
            code = type;
            url = uri;
            form = null;
            bytes = null;
            data = json;
            param = arg;
        }
    }

    public class ZWWWController : ZSingletonBehaviour<ZWWWController>
    {
        public int requestDataSize = 0;
        public int responseDataSize = 0;
        private Dictionary<int, UnityAction<ResponseInfo>> callFuns;
        public float delayTime = 30f;
        //public bool isGZIP = false;
        private string cookie = "";
        public string device = "";
        [SerializeField]
        private List<HttpInfo> requestList = null;
        private List<HttpInfo> loadingList = null;
        private List<HttpInfo> failedList = null;
        private bool isLoading = false;
        [SerializeField]
        private string currentReqURL = "";
        public string currentId = "";

        public static bool IsGZIP
        {
            get
            {
                return false;
            }
        }

        public static void AddListener(int id, UnityAction<ResponseInfo> callback)
        {
            Instance.AddListenser(id, callback);
        }

        public static void Load(string uid, int fun,int code, string url)
        {
            Instance.StartLoad(uid, fun,code, url);
        }

        public static void Load(string uid, int fun,int code, string url, byte[] bytes)
        {
            Instance.StartLoad(uid, fun,code, url, bytes);
        }

        public static void Load(string uid, int fun,int code, string url, WWWForm form,string param)
        {
            Instance.StartLoad(uid, fun,code, url, form,param);
        }

        public static void RemoveInfo()
        {
            Instance.RemoveHttpInfo();
        }

        public static bool AutoLoad()
        {
            return Instance.AutoLoadNext();
        }

        public static void UpdateHeaderInfo(string cookie)
        {
            Instance.cookie = cookie;
        }

        protected override void Awake()
        {
            base.Awake();
            requestList = new List<HttpInfo>();
            loadingList = new List<HttpInfo>();
            failedList = new List<HttpInfo>();
            callFuns = new Dictionary<int, UnityAction<ResponseInfo>>();
        }

        public void AddListenser(int id, UnityAction<ResponseInfo> callback)
        {
            if (callFuns.ContainsKey(id))
            {
                callFuns[id] = callback;
            }
            else
            {
                callFuns.Add(id, callback);
            }
        }

      

        public void StartLoad(string uid, int fun,int code, string url)
        {
            RemoveRequestInfo(uid);
            HttpInfo info = new HttpInfo(uid, fun,code, url,"");
            requestList.Add(info);
            StartLoadNext();
        }

        public void StartLoad(string uid, int fun,int code, string url, byte[] bytes)
        {
            RemoveRequestInfo(uid);
            HttpInfo info = new HttpInfo(uid, fun,code, url, bytes,"");
            requestList.Add(info);
            StartLoadNext();
        }

        public void StartLoad(string uid, int fun,int code, string url, WWWForm form,string param)
        {
            RemoveRequestInfo(uid);
            HttpInfo info = new HttpInfo(uid, fun,code, url, form, param);
            requestList.Add(info);
            StartLoadNext();
        }

        private void RemoveHttpInfo()
        {
            if (failedList.Count > 0)
                failedList.RemoveAt(failedList.Count - 1);
            if (requestList.Count > 0)
                requestList.RemoveAt(requestList.Count - 1);
        }

        private bool AutoLoadNext()
        {
            if (failedList.Count > 0)
            {
                HttpInfo info = failedList[0];
                
                StartLoad(info.uid, info.funID,info.code, info.url, info.form,info.param);
                failedList.RemoveAt(0);
                return true;
            }
            else
                return StartLoadNext();
        }

        private bool StartLoadNext()
        {
            if (isLoading)
                return false;
            else
            {
                if (requestList.Count > 0)
                {
                    HttpInfo info = requestList[0];
                    currentReqURL = info.url;
                    currentId = info.uid;
                    ZLog.Log("Request Server---...code = " + info.code + " --- url = " + info.url);
                    if (Application.internetReachability == NetworkReachability.NotReachable)
                    {
                        RequestException(HttpStateType.NonExistent);
                        return false;
                    }
                    isLoading = true;
                    
                    WWW www = null;

                    if (info.form != null)
                    {
                        Dictionary<string, string> headers = info.form.headers;
                        headers.Add("Device", device);
                        if (string.IsNullOrEmpty(cookie) == false)
                            headers.Add("Cookie", cookie);
                        //foreach (KeyValuePair<string, string> pair in headers)
                        //{
                        //    VLog.Warning(pair.Key + "--" + pair.Value);
                        //}
                        //Debug.LogWarning("WWWLoading...." + headers.ContainsKey("Cookie") + "---" + info.url + "---" + device + "---" + info.form.data.Length);
                        www = new WWW(info.url, info.form.data, headers);
                        requestDataSize += info.form.data.Length;
                    }
                    else if (info.bytes != null)
                    {
                        www = new WWW(info.url, info.bytes);
                    }
                    else
                    {
                        www = new WWW(info.url);
                    }
                    StartCoroutine(LoadingInspector(www,info));
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void RequestException(HttpStateType type)
        {
            HttpInfo info;
            if (!string.IsNullOrEmpty(currentId))
                info = GetRequestByUID(currentId);
            else
                info = GetRequestByURL(currentReqURL);
            if (info == null || (string.IsNullOrEmpty(info.uid) == false && callFuns.ContainsKey(info.funID)))
            {
                RemoveRequestInfo(info.uid);
                UnityAction<ResponseInfo> callfun = callFuns[info.funID];
                if (callfun != null)
                {
                    ResponseInfo response = new ResponseInfo();
                    response.state = type;
                    response.data = "Request that code is " + info.uid + " and url = " + info.url + " is " + type + "!!!";
                    response.param = info.param;
                    ZLog.Warning("RequestException....." + response.data);
                    callfun.Invoke(response);
                }
            }
            else
            {
                RemoveRequestInfo(info.uid);
            }
            currentReqURL = "";
            currentId = "";
            isLoading = false;
        }

        IEnumerator LoadingInspector(WWW www,HttpInfo info)
        {
            float counter = 0;
            bool timeOut = false;
            while (!www.isDone)
            {
                counter += Time.deltaTime;
                if (timeOut == false && counter > delayTime)
                {
                    timeOut = true;
                    isLoading = false;
                    counter = 0;
                    RequestException(HttpStateType.TimeOut);
                    yield break;
                }
                else
                    yield return null;
            }
         
            if (string.IsNullOrEmpty(info.uid) == false)
            {
                RemoveRequestInfo(info.uid);
                if (callFuns.ContainsKey(info.funID))
                {
                    UnityAction<ResponseInfo> callfun = callFuns[info.funID];
                    ResponseInfo response = new ResponseInfo();
                    response.code = info.code;
                    response.param = info.param;
                    if (callfun != null)
                    {
                        if (!string.IsNullOrEmpty(www.error))
                        {
                            ZLog.Warning("Http exception...." + info.uid + "---" + www.error);
                            response.state = HttpStateType.Exception;
                            response.data = www.error;
                           
                            callfun.Invoke(response);
                        }
                        else
                        {
                            string mess = www.text;
                            responseDataSize += mess.Length;
                            //if(isParseJson){
                            //    float diff = Time.realtimeSinceStartup;
                            //    //VLog.Warning("Parse json start....."+Time.realtimeSinceStartup);
                            //    ParseJsonTask task = new ParseJsonTask(mess, ParsJsonHandler);
                            //    new Thread(task.Parse).Start();
                            //    yield return null;
                            //    while (task.isParsing)
                            //    {
                            //        yield return null;
                            //    }
                            //    //VLog.Warning("Parse json end....." + (Time.realtimeSinceStartup - diff) + "s ----" + mess.Length);
                            //    response.jsonDic = task.result;
                            //}
                            //yield return null;
                           
                            Dictionary<string, string> header = www.responseHeaders;
                            if (header != null && header.ContainsKey("SET-COOKIE"))
                            {
                                response.cookie = header["SET-COOKIE"];
                            }
                            response.state = HttpStateType.Normal;
                            response.data = mess;
                            callfun.Invoke(response);
                        }
                    }
                }
                //VLog.Warning("load complelte!!!!" + requestDataSize + "---"+ responseDataSize);
            }
            yield return null;
            www.Dispose();
            currentId = "";
            isLoading = false;
            StartLoadNext();
            
        }

        public HttpInfo GetRequestByUID(string uid)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].uid == uid)
                    return requestList[i];
            }
            return null;
        }

        public HttpInfo GetLoadingByUID(string uid)
        {
            for (int i = 0; i < loadingList.Count; i++)
            {
                if (loadingList[i].uid == uid)
                    return loadingList[i];
            }
            return null;
        }

        public HttpInfo GetRequestByURL(string url)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].url == url)
                    return requestList[i];
            }
            return null;
        }

        public void RemoveLoadingInfo(string uid)
        {
            for (int i = 0; i < loadingList.Count; i++)
            {
                if (loadingList[i].uid == uid)
                {
                    loadingList.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveFailedInfo(string uid)
        {
            for (int i = 0; i < failedList.Count; i++)
            {
                if (failedList[i].uid == uid)
                {
                    failedList.RemoveAt(i);
                    break;
                }
            }
        }

        public void RemoveRequestInfo(string uid)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].uid == uid)
                {
                    requestList.RemoveAt(i);
                    break;
                }
            }
        }

        public static void AddListener(int p, object DoRequestComplete)
        {
            throw new System.NotImplementedException();
        }
    }
}
