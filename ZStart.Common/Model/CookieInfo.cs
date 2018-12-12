using System.Collections.Generic;
namespace ZStart.Common.Model
{
    public class CookieInfo
    {
        public string name = "";
        public string logoPath = "";
        public string logoUrl = "";
        public string version = "";
        public uint build = 0;
        public string package = "";
        public Dictionary<string,string> screenshots = null;
        public uint downloadPriority = 0;
        public int status = 0;
        public string auther = "";
        public uint size = 0;
        public bool isNew = false;
        public CookieInfo()
        {
            screenshots = new Dictionary<string, string>();
        }

        public void AddScreenshot(string url, string path)
        {
            if (screenshots.ContainsKey(url))
                screenshots[url] = path;
            else
                screenshots.Add(url,path);
        }

        public string GetScreenshot(int index)
        {
            if (screenshots.Count < 1)
                return "";
            int i = 0;
            foreach(KeyValuePair<string,string> pair in screenshots){
                if (i == index)
                    return pair.Value;
                i++;
            }
            return "";
        }

        public string GetScreenshot(string url)
        {
           if(screenshots.ContainsKey(url))
               return screenshots[url];
           return "";
        }

        public void RemoveScreenshot(string url)
        {
            if (screenshots.ContainsKey(url))
                screenshots.Remove(url);
        }

        public List<string> GetScreenshotUrls()
        {
            List<string> list = new List<string>();
            foreach(KeyValuePair<string,string> pair in screenshots){
                list.Add(pair.Key);
            }
            return list;
        }

        public List<string> GetScreenshotPaths()
        {
            List<string> list = new List<string>();
            foreach (KeyValuePair<string, string> pair in screenshots)
            {
                list.Add(pair.Value);
            }
            return list;
        }

        public override string ToString()
        {
            return "cookie info : name = " + name + ";package = " + package + ";logo path = " + logoPath;
        }
    }
}
