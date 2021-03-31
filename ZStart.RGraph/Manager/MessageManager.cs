using System;
using System.IO;
using UnityEngine;
using ZStart.Core;
using ZStart.Core.Controller;
using ZStart.RGraph.Enum;
using ZStart.RGraph.Util;

namespace ZStart.RGraph.Manager
{
    public class MessageManager
    {
        public static bool HttpConnected = false;

        protected static readonly string URL_Graph = "app/v1/graph";
        protected static readonly string URL_Entity = "app/v1/entity";
        protected static readonly string URL_Themes = "app/v1/themes";

        private static MessageManager mInstance = null;

        public static MessageManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new MessageManager();
                return mInstance;
            }
        }
        public int reqNum = 0;
        public int maxReconnected = 5;
        private MessageManager()
        {
            
        }

        protected string cloudAddress = "";
        public string CloudAddress
        {
            get
            {
                return cloudAddress;
            }
        }

        private string codes = "";
        private bool isLocal = true;
        private string rootPath = "";
        public string RootPath
        {
            get
            {
                return rootPath;
            }
        }
        public void SetInfo(string address, bool local, string root)
        {
            rootPath = root;
            cloudAddress = address;
            isLocal = local;
        }

        public void SetActiveCodes(string code)
        {
            codes = code;
        }

        public string GetAddress(HttpAPIType type)
        {
            string url = cloudAddress + "/";
            if (type == HttpAPIType.Graph)
            {
                return url + URL_Graph;
            }
            else if (type == HttpAPIType.Entity)
            {
                return url + URL_Entity;
            }
            else if (type == HttpAPIType.Themes)
            {
                return url + URL_Themes;
            }
            else
                return url;
        }

        private string DealResponse(string json, out int st)
        {
            if (string.IsNullOrEmpty(json))
            {
                st = (int)ServerErrorStaus.Unknown;
                DFNotifyManager.SendNotify(DFNotifyType.OnResponseError, ServerErrorStaus.Unknown);
                return "";
            }
            //ZLog.Log("DealResponse...." + json.Length);
            try
            {
                var obj = LitJson.JsonMapper.ToObject(json);
                st = (int)obj["status"];
                var status = (ServerErrorStaus)st;

                if (status == 0)
                {
                    var data = obj["data"];
                    return data.ToString();
                }
                else
                {
                    string msg = (string)obj["msg"];
                    ZLog.Warning("Server Response Exception...." + msg);
                    DFNotifyManager.SendNotify(DFNotifyType.OnResponseError, status);
                    return "";
                }
            }
            catch (Exception e)
            {
                ZLog.Warning(e.Message);
                st = -1;
                return "";
            }
        }

        private void HttpCompleteHandler(ZHttpController.Response info)
        {
            if (info.state != ZHttpController.Status.Normal)
            {
                reqNum += 1;
                if (reqNum > maxReconnected)
                {
                    MessageManager.HttpConnected = false;
                    ZHttpController.Clear();
                }
                ZLog.Warning("Http Response Exception...." + info.data);
                DFNotifyManager.SendNotify(DFNotifyType.OnResponseError, ServerErrorStaus.Success);
                return;
            }
            MessageManager.HttpConnected = true;
          
            int status = 0;
            string result = DealResponse(info.data, out status);
            ZLog.Log("HttpCompleteHandler....uid = " + info.uid + " ;result = " + result);
            if (status != 0)
                return;
            if (info.uid == URL_Graph)
            {
                Model.GraphModel graph = ParseUtil.ParseGraphJson(result, rootPath);
                DFNotifyManager.SendNotify(DFNotifyType.OnGraphDataUpdate, graph);
            }
            else if (info.uid == URL_Entity)
            {
                Model.EntityInfo tmp = ParseUtil.ParseEntityJson(result,"");
                DFNotifyManager.SendNotify(DFNotifyType.OnEntityDataUpdate, tmp);
            }else if (info.uid == URL_Themes)
            {
                // DFNotifyManager.SendNotify(DFNotifyType.OnThemesUpdate, list);
            }
        }

        public void RequestGraph(string uid, string parent)
        {
            if (isLocal)
            {
                var path = Path.Combine(rootPath, "graph/graph_" + uid + ".json");
                if (!File.Exists(path))
                {
                    ZLog.Warning("the file not exist that path = " + path);
                    DFNotifyManager.SendNotify(DFNotifyType.OnGraphDataUpdate, null);
                    return;
                }
                var txt = File.ReadAllText(path);
                var graph = ParseUtil.ParseGraphJson(txt, rootPath);
                if (graph == null)
                {
                    ZLog.Warning("the graph data is null");
                    DFNotifyManager.SendNotify(DFNotifyType.OnGraphDataUpdate, null);
                    return;
                }
                graph.Parent = parent;
                DFNotifyManager.SendNotify(DFNotifyType.OnGraphDataUpdate, graph);
            }
            else
            {
                string address = GetAddress(HttpAPIType.Graph);
                var json = new LitJson.JsonData
                {
                    ["uid"] = uid ,
                    ["parent"] = parent,
                    ["code"] = codes
                 };
                ZHttpController.Post(URL_Graph, address, json.ToString(), HttpCompleteHandler, true);
            }
        }

        public void RequestGraphByTheme(string uid)
        {
            if (isLocal)
            {
                return;
            }
            else
            {
                string address = GetAddress(HttpAPIType.Graph);
                LitJson.JsonData json = new LitJson.JsonData
                {
                    ["theme"] = uid,
                    ["code"] = codes
                };
                ZHttpController.Post(URL_Graph, address, json.ToString(), HttpCompleteHandler, true);
            }
        }

        public void RequestEntity(string uid)
        {
            if (isLocal)
            {
                var path = Path.Combine(rootPath, "entity/entity_" + uid + ".json");
                if (!File.Exists(path))
                {
                    ZLog.Warning("the file not exist that path = " + path);
                    return;
                }
                var txt = File.ReadAllText(path);
                var info = ParseUtil.ParseEntityJson(txt, rootPath);
                DFNotifyManager.SendNotify(DFNotifyType.OnEntityDataUpdate, info);
            }
            else
            {
                string address = GetAddress(HttpAPIType.Entity);
                LitJson.JsonData json = new LitJson.JsonData
                {
                    ["uid"] = uid,
                    ["code"] = codes
                };
                ZHttpController.Post(URL_Entity, address, json.ToString(), HttpCompleteHandler, true);
            }
            
        }

        public void RequestThemes()
        {
            if (isLocal)
            {
                var path = Path.Combine(rootPath, "themes.json");
                if (!File.Exists(path))
                {
                    ZLog.Warning("the file not exist that path = " + path);
                    return;
                }
                var txt = File.ReadAllText(path);
                var list = ParseUtil.ParseThemes(txt, rootPath);
                DFNotifyManager.SendNotify(DFNotifyType.OnThemesUpdate, list);
            }
            else
            {
                string address = GetAddress(HttpAPIType.Themes);
                var json = new LitJson.JsonData
                {
                    ["code"] = codes
                };
                ZHttpController.Post(URL_Themes, address, json.ToString(), HttpCompleteHandler, true);
            }
        }
    }
}
