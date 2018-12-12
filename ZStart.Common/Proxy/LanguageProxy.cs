using System.Collections.Generic;
using System.Xml;

namespace ZStart.Common.Proxy
{
    public class LanguageProxy:BaseProxy
    {
        private Dictionary<string, string> keyValues;
        public LanguageProxy()
        {
            keyValues = new Dictionary<string, string>();
        }

        public virtual void Parse(string xml)
        {
            if (keyValues.Count > 0)
                keyValues.Clear();
            XmlDocument languageDoc = new XmlDocument();
            languageDoc.LoadXml(xml);
            XmlNodeList list = languageDoc.SelectNodes("/resources/string");
            foreach (XmlNode node in list)
            {
                keyValues[node.Attributes["name"].Value] = node.FirstChild.InnerText.Trim();
            }
        }

        public string GetValue(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Empty;
            }
            if (keyValues.ContainsKey(key))
            {
                return keyValues[key];
            }
            return string.Empty;
        }

        public string GetValue(string key, params object[] args)
        {
            string val = GetValue(key);
            return string.Format(val, args);
        }
    }
}
