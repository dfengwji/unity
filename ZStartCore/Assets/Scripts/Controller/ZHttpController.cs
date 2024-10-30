using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace ZStart.Core.Controller
{
    public class ZHttpController : ZSingletonBehaviour<ZHttpController>
    {
        public enum Status
        {
            Normal = 0,
            NetworkError = 1,
            ServerError = 2,
            TimeOut = 3,
            NotExist = 4,
        }

        public class Response
        {
            public Status state;
            public string uid;
            public string token;
            public string error;
            public string data;
            public string param;
        }

        public class Request
        {
            public string uid;
            public string url;
            public WWWForm form;
            public string data;
            public string param;
            public string method = UnityWebRequest.kHttpVerbPOST;
            public bool isLoop = false;
            public bool isBase64 = false;
            public bool isLog = false;
            public UnityAction<Response> callFun;

            public Request(string id, string uri, string arg, UnityAction<Response> call, bool loop, bool base64, bool log)
            {
                uid = id;
                callFun = call;
                url = uri;
                form = null;
                data = "";
                param = arg;
                isLoop = loop;
                isBase64 = base64;
                isLog = log;
            }

            public Request(string id, string uri, WWWForm f, string arg, UnityAction<Response> call, bool loop, bool base64, bool log)
            {
                uid = id;
                callFun = call;
                url = uri;
                form = f;
                data = "";
                param = arg;
                isLoop = loop;
                isBase64 = base64;
                isLog = log;
            }

            public Request(string id, string uri, string json, string arg, UnityAction<Response> call, bool loop, bool base64, bool log)
            {
                uid = id;
                callFun = call;
                url = uri;
                form = null;
                data = json;
                param = arg;
                isLoop = loop;
                isBase64 = base64;
                isLog = log;
            }
        }

        public int requestDataSize = 0;
        public int responseDataSize = 0;
        public int delayTime = 30;
        //public bool isGZIP = false;
        private string Token = "";
        public string device = "";
        public bool base64 = true;
        [SerializeField]
        private List<Request> requestList = null;
        private List<Request> loadingList = null;
        private List<Request> failedList = null;
        public bool isLoading = false;
        public bool isLog = false;

        [SerializeField]
        private string currentReqURL = "";

        public static bool IsGZIP
        {
            get
            {
                return false;
            }
        }

        public static void Post(string uid, string url, UnityAction<Response> call, bool loop = false, bool b64 = false, bool log = false)
        {
            Instance.StartLoad(uid, url, call, UnityWebRequest.kHttpVerbPOST, loop, b64, log);
        }

        public static void Post(string uid, string url, string json, UnityAction<Response> call, bool loop = false, bool b64 = false, bool log = false)
        {
            Instance.StartLoad(uid, url, json, "", call, loop, b64, log);
        }

        public static void Post(string uid, string url, WWWForm form, string param, UnityAction<Response> call, bool loop = false, bool b64 = false, bool log = false)
        {
            Instance.StartLoad(uid, url, form, param, call, loop, b64, log);
        }

        public static void Get(string uid, string url, UnityAction<Response> call, bool loop = false, bool b64 = false, bool log = false)
        {
            Instance.StartLoad(uid, url, call, UnityWebRequest.kHttpVerbGET, loop, b64, log);
        }

        public static void RemoveInfo()
        {
            Instance.RemoveHttpInfo();
        }

        public static bool AutoLoad()
        {
            return Instance.AutoLoadNext();
        }

        public static void UpdateHeaderInfo(string token)
        {
            Instance.Token = token;
        }

        public static void Clear()
        {
            Instance.ClearAll();
        }

        protected override void Awake()
        {
            base.Awake();
            requestList = new List<Request>();
            loadingList = new List<Request>();
            failedList = new List<Request>();
        }

        protected void StartLoad(string uid, string url, UnityAction<Response> call, string method, bool loop, bool b64, bool log)
        {
            RemoveRequestInfo(uid);
            Request info = new Request(uid, url, "", call, loop, b64, log)
            {
                method = method,
                isBase64 = b64,
            };
            requestList.Add(info);
            StartLoadNext();
        }

        protected void StartLoad(string uid, string url, string json, string param, UnityAction<Response> call, bool loop, bool b64, bool log)
        {
            RemoveRequestInfo(uid);
            Request info = new Request(uid, url, json, param, call, loop, b64, log)
            {
                method = UnityWebRequest.kHttpVerbPOST,
            };
            requestList.Add(info);
            StartLoadNext();
        }

        public void StartLoad(string uid, string url, WWWForm form, string param, UnityAction<Response> call,bool loop, bool b64, bool log)
        {
            RemoveRequestInfo(uid);
            Request info = new Request(uid, url, form, param, call, loop, b64, log)
            {
                method = UnityWebRequest.kHttpVerbPOST,
            };
            requestList.Add(info);
            StartLoadNext();
        }

        private void ClearAll()
        {
            failedList.Clear();
            requestList.Clear();
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
                Request info = failedList[0];
                StartLoad(info.uid, info.url, info.form, info.param, info.callFun, info.isLoop, info.isBase64, info.isLog);
                failedList.RemoveAt(0);
                return true;
            }
            else
                return StartLoadNext();
        }

        private void RequestException(Request info, Status type, string msg)
        {
            if (info != null)
            {
                if (info.callFun != null)
                {
                    Response response = new Response
                    {
                        uid = info.uid,
                        state = type,
                        data = "Request that code is " + info.uid + " and url = " + info.url + " ;msg =  " + msg + "!!!",
                        param = info.param,
                    };
                    Debug.LogError("RequestException....." + response.data);
                    info.callFun.Invoke(response);
                }
            }
            else
            {
                Debug.LogError("RequestException.....HttpState = " + type + "---" + msg);
            }

            currentReqURL = "";
            isLoading = false;
        }

        private bool StartLoadNext()
        {
            if (isLoading)
                return false;
            if (requestList.Count > 0)
            {
                Request info = requestList[0];
                currentReqURL = info.url;

                isLoading = true;

                if (info.method == UnityWebRequest.kHttpVerbGET)
                {
                    StartCoroutine(LoadingInspector(info, UnityWebRequest.kHttpVerbGET, false));
                }
                else
                {
                    if (info.form != null)
                    {
                        requestDataSize += info.form.data.Length;
                        StartCoroutine(LoadingInspector(info, UnityWebRequest.kHttpVerbPOST, false));
                    }
                    else
                    {
                        string json = "";
                        if (info.isBase64)
                        {
                            byte[] bt = Encoding.Default.GetBytes(info.data);
                            json = Convert.ToBase64String(bt);
                        }
                        else
                        {
                            json = info.data;
                        }
                        info.data = json;
                        StartCoroutine(LoadingInspector(info, UnityWebRequest.kHttpVerbPOST, true));
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        IEnumerator CheckNetworkDelay()
        {
            yield return new WaitForSeconds(3f);
            StartLoadNext();
        }

        private UnityWebRequest CreateRequest(Request info, string method, bool json)
        {
            UnityWebRequest request;
            if (method == UnityWebRequest.kHttpVerbGET)
            {
                request = UnityWebRequest.Get(info.url);
            }else
            {
                if (json)
                {
                    request = new UnityWebRequest(info.url, UnityWebRequest.kHttpVerbPOST)
                    {
                        downloadHandler = new DownloadHandlerBuffer()
                    };
                    byte[] bts = System.Text.Encoding.UTF8.GetBytes(info.data);
                    request.uploadHandler = new UploadHandlerRaw(bts);
                    request.uploadHandler.contentType = "application/json;charset=utf-8";
                }
                else
                {
                    request = UnityWebRequest.Post(info.url, info.form);
                }
            }
            return request;
        }

        IEnumerator LoadingInspector(Request info, string method, bool json)
        {
            if (info.isLog)
                Debug.Log(string.Format("Request Server...uid = {0};method = {1} ; url = {2};  data ={3}", info.uid, method, info.url, info.data));

            using (UnityWebRequest request = CreateRequest(info, method, json))
            {
                //request.SetRequestHeader("Content-Type", "application/json");

                //if (!string.IsNullOrEmpty(Token))
                // request.SetRequestHeader("Bearer Token", "Bearer " + Token);
                request.timeout = delayTime;
                yield return request.SendWebRequest();

                if (!string.IsNullOrEmpty(request.error))
                {
                    RequestException(info, Status.ServerError, request.error);
                    if (!info.isLoop)
                    {
                        if (info.isLog)
                            Debug.Log("no loop the remove the request that id = " + info.uid);
                        RemoveRequestInfo(info.uid);
                    }
                }
                else
                {
                    Response response = new Response
                    {
                        uid = info.uid,
                        param = info.param
                    };

                    if (info.callFun != null)
                    {
                        if (!string.IsNullOrEmpty(request.error))
                        {
                            Debug.LogError("Http exception...." + info.uid + "---" + request.error);
                            response.state = Status.ServerError;
                            response.data = request.error;

                            info.callFun.Invoke(response);
                        }
                        else
                        {
                            string mess = DownloadHandlerBuffer.GetContent(request);
                            responseDataSize += mess.Length;

                            response.state = Status.Normal;
                            response.data = mess;
                            info.callFun.Invoke(response);
                        }
                    }
                    RemoveRequestInfo(info.uid);
                }
                // yield return null;
                request.Dispose();
            }
            yield return null;
            isLoading = false;
            StartLoadNext();
        }

        public Request GetRequestByUID(string uid)
        {
            for (int i = 0; i < requestList.Count; i++)
            {
                if (requestList[i].uid == uid)
                    return requestList[i];
            }
            return null;
        }

        public Request GetLoadingByUID(string uid)
        {
            for (int i = 0; i < loadingList.Count; i++)
            {
                if (loadingList[i].uid == uid)
                    return loadingList[i];
            }
            return null;
        }

        public Request GetRequestByURL(string url)
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
    }
}
