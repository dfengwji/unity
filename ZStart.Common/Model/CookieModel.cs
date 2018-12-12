using ZStart.Core.Common;
using ZStart.Core.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ZStart.Core;

namespace ZStart.Common.Model
{
    public class CookieModel
    {
        private List<CookieInfo> infoList;

        /// <summary>
        /// {"apps":[{"package":"","uid":"","name":"","logo":"","images":[{"url":"","path":""},...],"version":"","build":1,"auther":"","size":0,"prior":0,"status":0},...]}
        /// </summary>
        /// 
        private uint downloadPriority = 0;
        public CookieModel()
        {
            infoList = new List<CookieInfo>();
        }

        private void Parse(string json)
        {
            infoList.Clear();
            if(string.IsNullOrEmpty(json))
                return;
            try
            {
                Dictionary<string, object> jsonDic = FastJson.Deserialize(json) as Dictionary<string, object>;
                List<object> list = jsonDic["apps"] as List<object>;
                if (list == null)
                    return;
                for (int i = 0; i < list.Count; i++)
                {
                    Dictionary<string, object> dic = list[i] as Dictionary<string, object>;
                    CookieInfo info = new CookieInfo();
                    info.package = dic.ContainsKey("pkg") ? dic["pkg"].ToString() : "";
                    info.name = dic.ContainsKey("name") ? dic["name"].ToString() : "";
                    //info.logoPath = dic.ContainsKey("path") ? dic["path"].ToString() : "";
                    info.logoUrl = dic.ContainsKey("url") ? dic["url"].ToString() : "";
                    info.version = dic.ContainsKey("ver") ? dic["ver"].ToString() : "";
                    info.build = dic.ContainsKey("build") ? uint.Parse(dic["build"].ToString()) : 0;
                    info.auther = dic.ContainsKey("auther") ? dic["auther"].ToString() : "";
                    info.size = dic.ContainsKey("size") ? uint.Parse(dic["size"].ToString()) : 0;
                    //info.downloadPriority = dic.ContainsKey("down") ? uint.Parse(dic["down"].ToString()) : 0;
                    //info.status = dic.ContainsKey("st") ? int.Parse(dic["st"].ToString()) : 0;
                    List<object> images = dic.ContainsKey("images") ? dic["images"] as List<object> : null;
                    if (images != null)
                    {
                        for (int j = 0; j < images.Count; j++)
                        {
                            Dictionary<string, object> tmp = images[j] as Dictionary<string, object>;
                            info.AddScreenshot(tmp["url"].ToString(), tmp["path"].ToString());
                        }
                    }
                    if (info.downloadPriority > downloadPriority)
                        downloadPriority = info.downloadPriority;
                    //ZLog.Log("read cookie:"+info.ToString());
                    infoList.Add(info);
                }
            }catch(Exception e){
                Debug.LogException(e);
            }
        }

        public override string ToString()
        {
            StringBuilder json = new StringBuilder();
            json.Append("{\"apps\":[");
            for (int i = 0,max = infoList.Count; i < max; i++ )
            {
                CookieInfo info = infoList[i];
                StringBuilder app = new StringBuilder();
                app.Append("{");
                app.Append(JsonUtil.FormatJson("pkg", info.package) + ",");
                app.Append(JsonUtil.FormatJson("name", info.name) + ",");
                //app.Append(JsonUtil.FormatJson("path", info.logoPath) + ",");
                app.Append(JsonUtil.FormatJson("url", info.logoUrl) + ",");
                app.Append(JsonUtil.FormatJson("ver", info.version) + ",");
                app.Append(JsonUtil.FormatJson("build", info.build) + ",");
                app.Append(JsonUtil.FormatJson("auther", info.auther) + ",");
                app.Append(JsonUtil.FormatJson("size", info.size) + ",");
                //app.Append(JsonUtil.FormatJson("down", info.downloadPriority) + ",");
                //app.Append(JsonUtil.FormatJson("st", info.status) + ",");
                object images = JsonUtil.FormatJson<string, string>("images", "url", "path", info.screenshots);
                app.Append(images.ToString());

                if (i < max - 1)
                    json.Append(app.ToString() + "},");
                else
                    json.Append(app.ToString()+"}");
            }
            json.Append("]}");
            return json.ToString();
        }

        private void AddItem(CookieInfo info)
        {
            if (info == null || HasItem(info.package))
                return;
          
            infoList.Add(info);
        }

        public bool HasItem(string package)
        {
            for (int i = 0, max = infoList.Count; i < max; i++)
            {
                if (infoList[i].package == package)
                    return true;
            }
            return false;
        }

        public CookieInfo GetItem(string package)
        {
            for (int i = 0, max = infoList.Count; i < max; i++)
            {
                if (infoList[i].package == package)
                    return infoList[i];
            }
            return null;
        }

        public void Test()
        {
            infoList.Clear();
            for (int i = 0; i < 10; i++)
            {
                CookieInfo cookie = new CookieInfo();
                cookie.name = "testrobobobobob" + i;
                cookie.logoPath = "file://dfdsfdfdsfdsfdfdsfdsfs";
                cookie.size = 43353434;
                cookie.auther = "auther...oooooooo";
                cookie.AddScreenshot("url","path");
                cookie.AddScreenshot("url2", "path2");
                cookie.downloadPriority = 0;
                infoList.Add(cookie);
            }
            Save();
        }

        public List<CookieInfo> GetOldInfos()
        {
            List<CookieInfo> list = new List<CookieInfo>();
            for (int i = 0; i < infoList.Count;i++ )
            {
                if (infoList[i].isNew == false)
                    list.Add(infoList[i]);
            }
            for (int i = 0; i < list.Count; i++)
            {
                infoList.Remove(list[i]);
            }
            return list;
        }

        public void UpdateDownload(string package, bool waiting)
        {
            CookieInfo cookie = GetItem(package);
            if (cookie == null)
                return;
            if (waiting)
            {
                downloadPriority += 1;
                cookie.downloadPriority = downloadPriority;
            }
            else
            {
                cookie.downloadPriority = 0;
            }
        }

        public void UpdateStatus(string package, int status)
        {
            CookieInfo cookie = GetItem(package);
            if (cookie == null)
                return;
            cookie.status = status;
        }

        public void UpdateLogo(string package, string url, string path)
        {
            CookieInfo info = GetItem(package);
            if (info == null)
                return;
            info.logoUrl = url;
            info.logoPath = path;
        }

        public string CheckLogo(string package, string url)
        {
            CookieInfo info = GetItem(package);
            if (info == null)
                return "";
            if (info.logoUrl == url)
                return "";
            else
                return info.logoPath;
        }

        public List<string> UpdateScreenshots(string package, string[] urls)
        {
            if (urls == null || urls.Length < 1)
                return null;
            CookieInfo info = GetItem(package);
            if (info == null)
                return null;
            List<string> newUrls = new List<string>(urls);
            List<string> oldPaths = new List<string>();
            List<string> cacheUrls = info.GetScreenshotUrls();
            for (int i = 0; i < cacheUrls.Count;i++ )
            {
                string url = cacheUrls[i];
                if (newUrls.Contains(url) == false)
                {
                    oldPaths.Add(info.GetScreenshot(url));
                    info.RemoveScreenshot(url);
                }
            }
            return oldPaths;
        }

        public void UpdateScreenshot(string package, string url, string path)
        {
            CookieInfo info = GetItem(package);
            if (info == null)
                return;
            info.AddScreenshot(url, path);
        }

        private string GetCookiePath()
        {
            return Path.Combine(Application.persistentDataPath, "cookie.json");
        }

        public void Save()
        {
            //float diff = Time.realtimeSinceStartup;
            string json = ToString();
            //ZLog.Log("Write cookie.....middle---" + Time.realtimeSinceStartup+"---"+json.Length);
            AppContext.WriteString(AppContext.AppPrefEnum.Cookie,json);
            //FileCacheManager.Instance.WriteFile(GetCookiePath(), Encoding.UTF8.GetBytes(json));
            //ZLog.Log("Write cookie.... number =  "+infoList.Count);
        }

        public void Read()
        {
            //float diff = Time.realtimeSinceStartup;
            //string json = FileCacheManager.Instance.ReadText(GetCookiePath());
            string json = AppContext.ReadString(AppContext.AppPrefEnum.Cookie);
            //ZLog.Log("Read cookie.....start---"+json);
            Parse(json);
            ZLog.Log("Read cookie.....number = "+infoList.Count);
        }

        public List<string> GetWaitingList()
        {
            List<string> list = new List<string>();
            for (int i = 0, max = infoList.Count; i < max; i++)
            {
                if (infoList[i].downloadPriority > 0)
                {
                    list.Add(infoList[i].package);
                }
            }
            return list;
        }
    }
}
