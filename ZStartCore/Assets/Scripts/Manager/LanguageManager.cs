using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ZStart.Core;

namespace IVRApp.Core.Manager
{
    public class LanguageManager
    {
        public enum LanguageEnum
        {
            POPUPWIN,
            EXCEPTION,
        }

        public enum LanguageCountryEnum
        {
            ZH_CN,
            EN_US,
            JA_JP,
        }

        private static LanguageManager _instance = null;

        private Dictionary<string, string> _popupwinDic = null;
        private Dictionary<string, string> _exceptionDic = null;
   
        public LanguageCountryEnum country = LanguageCountryEnum.ZH_CN;
        private LanguageManager()
        {
            Init();
        }

        public static LanguageManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new LanguageManager();
                }
                return _instance;
            }
        }

        private void Init()
        {
            _popupwinDic = new Dictionary<string, string>();
            _exceptionDic = new Dictionary<string, string>();

            SystemLanguage language = Application.systemLanguage;
            if (language == SystemLanguage.Chinese || language == SystemLanguage.ChineseSimplified || language == SystemLanguage.ChineseTraditional)
            {
                SwitchLocal(LanguageCountryEnum.ZH_CN);
            }
            else if (language == SystemLanguage.Japanese)
            {
                SwitchLocal(LanguageCountryEnum.JA_JP);
            }
            else
            {
                SwitchLocal(LanguageCountryEnum.EN_US);
            }
        }

        public void Update()
        {

        }

        public static void Clear()
        {
            _instance = null;
        }

        public void SwitchLocal(LanguageCountryEnum code)
        {
            country = code;
            string prefix = GetPrefixPath(true);
            _popupwinDic = ReadLocalFile(prefix + "popupwin");
            _exceptionDic = ReadLocalFile(prefix + "exception");
          
        }

        private string GetPrefixPath(bool local)
        {
            if (local)
            {
                if (country == LanguageCountryEnum.ZH_CN)
                    return "local/zh_cn/";
                else if (country == LanguageCountryEnum.JA_JP)
                    return "local/ja_jp/";
                else
                    return "local/en_us/";
            }
            else
            {
                if (country == LanguageCountryEnum.ZH_CN)
                    return "assets/resources/local/zh_cn/";
                else if (country == LanguageCountryEnum.JA_JP)
                    return "assets/resources/local/ja_jp/";
                else
                    return "assets/resources/local/en_us/";
            }
        }

        private List<string> ReadLocalLines(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            return ParseLines(asset);
        }

        private List<string> ParseLines(TextAsset asset)
        {
            if (asset == null)
                return null;
            TextAssetReader reader = new TextAssetReader(asset);
            return reader.ReadLines();
        }

        private Dictionary<string, string> ReadLocalFile(string path)
        {
            TextAsset asset = Resources.Load<TextAsset>(path);
            return ParseFile(asset);
        }

        private Dictionary<string, string> ParseFile(TextAsset asset)
        {
            if (asset == null)
                return null;
            TextAssetReader reader = new TextAssetReader(asset);
            return reader.Read();
        }

        private Dictionary<string, string> GetDicByType(LanguageEnum type)
        {
            if (type == LanguageEnum.EXCEPTION)
            {
                return _exceptionDic;
            }
            else if (type == LanguageEnum.POPUPWIN)
            {
                return _popupwinDic;
            }
         
            return null;
        }

        private string GetString(string key, LanguageEnum type)
        {
            Dictionary<string, string> dic = GetDicByType(type);
            if (dic == null)
                throw new Exception("locale language manager can not find file!!!");
            foreach (KeyValuePair<string, string> pair in dic)
            {
                if (pair.Key.CompareTo(key) == 0)
                {
                    return pair.Value;
                }
            }
            return null;
        }

        public void UpateLanguage(AssetBundle bundle)
        {
            if (bundle == null)
                return;
            string prefix = GetPrefixPath(false);
            if (bundle.Contains(prefix + "popupwin.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "popupwin.txt");
                if (asset != null)
                    _popupwinDic = ParseFile(asset);
            }
            if (bundle.Contains(prefix + "exception.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "exception.txt");
                if (asset != null)
                    _exceptionDic = ParseFile(asset);
            }
        }

        private string GetString(string key, LanguageEnum type, params object[] args)
        {
            string val = GetString(key, type);
            if (val == null)
            {
                ZLog.Warning("can not find the key = " + key + " in local language!!!!");
                return "";
            }
            return string.Format(val, args);
        }

        public static string GetPopupwin(string key, params object[] args)
        {
            return Instance.GetString(key, LanguageEnum.POPUPWIN, args);
        }

        public static string GetPopupwin(string key)
        {
            return Instance.GetString(key, LanguageEnum.POPUPWIN);
        }

        public static string GetException(string key, params object[] args)
        {
            return Instance.GetString(key, LanguageEnum.EXCEPTION, args);
        }
    }
    class TextAssetReader
    {
        private byte[] mBuffer;
        private int mOffset = 0;
        public TextAssetReader(TextAsset asset)
        {
            mBuffer = asset.bytes;
        }

        private bool canRead { get { return (mBuffer != null && mOffset < mBuffer.Length); } }

        private string ReadLine(byte[] buffer, int start, int count)
        {
            return Encoding.UTF8.GetString(buffer, start, count);
        }

        private string ReadLine()
        {
            int max = mBuffer.Length;

            // Skip empty characters
            while (mOffset < max && mBuffer[mOffset] < 32) ++mOffset;

            int end = mOffset;

            if (end < max)
            {
                for (; ; )
                {
                    if (end < max)
                    {
                        int ch = mBuffer[end++];
                        if (ch != '\n' && ch != '\r') continue;
                    }
                    else ++end;

                    string line = ReadLine(mBuffer, mOffset, end - mOffset - 1);
                    mOffset = end;
                    return line;
                }
            }
            mOffset = max;
            return null;
        }

        public List<string> ReadLines()
        {
            List<string> list = new List<string>();
            while (canRead)
            {
                string line = ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                if (list.Contains(line))
                    Debug.LogError("the charactors is echo in local file!!! please check the charactors = " + line);
                else
                    list.Add(line);
            }
            return list;
        }

        public Dictionary<string, string> Read()
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            char[] separator = new char[] { '=' };

            while (canRead)
            {
                string line = ReadLine();
                if (string.IsNullOrEmpty(line)) break;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);

                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim();
                    if (val.Contains("\\"))
                    {
                        string[] array = val.Split(new char[] { '\\'});
                        StringBuilder tmp = new StringBuilder();
                        for (int i = 0; i < array.Length; i++)
                        {
                            string str = array[i];
                            if (string.IsNullOrEmpty(str) == false)
                            {
                                if (i < array.Length - 1)
                                    tmp.Append(str + Environment.NewLine);
                                else
                                    tmp.Append(str);
                            }
                        }
                        val = tmp.ToString();
                    }
                    if (dict.ContainsKey(key))
                        Debug.LogError("the key is echo in local file!!! please check the key = " + key);
                    else
                        dict.Add(key, val);
                }
            }
            return dict;
        }
    }
}
