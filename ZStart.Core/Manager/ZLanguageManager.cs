using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZStart.Core.Manager
{
    public class ZLanguageManager
    {
        public enum LanguageEnum
        {
		    POPUPWIN,
		    EXCEPTION,
		    PROP
	    }

        public enum LanguageCountryEnum
        {
            ZH_CN,
            EN_US
        }
	
	    private static ZLanguageManager _instance = null;
	
	    private Dictionary<string,string> _popupwinDic = null;
	    private Dictionary<string,string> _exceptionDic = null;
	    private Dictionary<string, string> _itemDic = null;

        private List<string> _familyNames = null;
        private List<string> _boyNames = null;
        private List<string> _girlNames = null;
        private List<string> _sensitiveWords = null;
        public LanguageCountryEnum country = LanguageCountryEnum.ZH_CN;
	    private ZLanguageManager ()
	    {
            Init();
        }
	
	    public static ZLanguageManager Instance
	    {
            get
            {
                if (_instance == null)
                {
                    _instance = new ZLanguageManager();
                }
                return _instance;
            }
	    }

        private void Init()
        {
            _popupwinDic = new Dictionary<string, string>();
            _exceptionDic = new Dictionary<string, string>();
            _itemDic = new Dictionary<string, string>();
            _familyNames = new List<string>();
            _boyNames = new List<string>();
            _girlNames = new List<string>();
            _sensitiveWords = new List<string>();
            SwitchLocal(LanguageCountryEnum.ZH_CN);
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
            _itemDic = ReadLocalFile(prefix + "prop");
            _familyNames = ReadLocalLines(prefix + "name_family");
            _boyNames = ReadLocalLines(prefix + "name_boy");
            _girlNames = ReadLocalLines(prefix + "name_girl");
            _sensitiveWords = ReadLocalLines(prefix + "sensitive");
        }

        private string GetPrefixPath(bool local)
        {
            if (local)
            {
                if (country == LanguageCountryEnum.ZH_CN)
                    return "local/zh_cn/";
                else
                    return "local/en_us/";
            }
            else
            {
                if (country == LanguageCountryEnum.ZH_CN)
                    return "assets/resources/local/zh_cn/";
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

	    private Dictionary<string,string> GetDicByType(LanguageEnum type)
	    {
		    if(type == LanguageEnum.EXCEPTION){
			    return _exceptionDic;
		    }else if(type == LanguageEnum.POPUPWIN){
			    return _popupwinDic;
		    }else if(type == LanguageEnum.PROP){
			    return _itemDic;
		    }
		    return null;
	    }
	
	    private string GetString(string key,LanguageEnum type)
	    {
		    Dictionary<string,string> dic = GetDicByType(type);
		    if(dic == null)
			    throw new Exception("locale language manager can not find file!!!");
		    foreach(KeyValuePair<string,string> pair in dic){
			    if(pair.Key.CompareTo(key) == 0){
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
            if (bundle.Contains(prefix+"popupwin.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "popupwin.txt");
                if (asset != null)
                    _popupwinDic = ParseFile(asset);
            }
            if (bundle.Contains(prefix+ "exception.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "exception.txt");
                if (asset != null)
                    _exceptionDic = ParseFile(asset);
            }
            if (bundle.Contains(prefix+ "item.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "item.txt");
                if (asset != null)
                    _itemDic = ParseFile(asset);
            }
            if (bundle.Contains(prefix+ "name_family.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "name_family.txt");
                if (asset != null)
                    _familyNames = ParseLines(asset);
            }
            if (bundle.Contains(prefix+ "name_boy.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "name_boy.txt");
                if (asset != null)
                    _boyNames = ParseLines(asset);
            }
            if (bundle.Contains(prefix+ "name_girl.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "name_girl.txt");
                if (asset != null)
                    _girlNames = ParseLines(asset);
            }
            if (bundle.Contains(prefix+ "sensitive.txt"))
            {
                TextAsset asset = bundle.LoadAsset<TextAsset>(prefix + "sensitive.txt");
                if (asset != null)
                    _sensitiveWords = ParseLines(asset);
            }
        }
	
	    private string GetString(string key,LanguageEnum type,params object[] args)
	    {
		    string val = GetString(key,type);
		    if(val == null) {
                ZLog.Warning("can not find the key = "+key+" in local language!!!!");
                return "";
            }
		    return string.Format(val,args);
	    }

        private string RandomName(bool boy)
        {
            int first = UnityEngine.Random.Range(0,_familyNames.Count);
            string last = "";
            if (boy)
            {
                int tmp = UnityEngine.Random.Range(0, _boyNames.Count);
                last = _boyNames[tmp];
            }
            else
            {
                int girl = UnityEngine.Random.Range(0, _girlNames.Count);
                last = _girlNames[girl];
            }

            return _familyNames[first]+last;
        }

        private string RandomName()
        {
            int first = UnityEngine.Random.Range(0, _familyNames.Count);
            string last = "";
            if (UnityEngine.Random.Range(0, 100) > 50)
            {
                int boy = UnityEngine.Random.Range(0, _boyNames.Count);
                last = _boyNames[boy];
            }
            else
            {
                int girl = UnityEngine.Random.Range(0, _girlNames.Count);
                last = _girlNames[girl];
            }

            return _familyNames[first] + last;
        }

        private bool HasSensitive(string words)
        {
            if (_sensitiveWords.Contains(words))
                return true;
            else
                return false;
        }

        public static string GetRandomName(bool boy)
        {
            return Instance.RandomName(boy);
        }

        public static string GetRandomName()
        {
            return Instance.RandomName();
        }

        public static bool IsSensitiveWords(string words)
        {
            return Instance.HasSensitive(words);
        }

	    public static string GetGameProp(string key, params object[] args)
	    {
            return Instance.GetString(key, LanguageEnum.PROP, args);
	    }
	
	    public static string GetPopupwin(string key,params object[] args)
	    {
		    return Instance.GetString(key,LanguageEnum.POPUPWIN,args);
	    }

        public static string GetPopupwin(string key)
        {
            return Instance.GetString(key, LanguageEnum.POPUPWIN);
        }

	    public static string GetException(string key,params object[] args)
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
    #if UNITY_FLASH
		    // Encoding.UTF8 is not supported in Flash :(
		    StringBuilder sb = new StringBuilder();

		    int max = start + count;

		    for (int i = start; i < max; ++i)
		    {
			    byte byte0 = buffer[i];

			    if ((byte0 & 128) == 0)
			    {
				    // If an UCS fits 7 bits, its coded as 0xxxxxxx. This makes ASCII character represented by themselves
				    sb.Append((char)byte0);
			    }
			    else if ((byte0 & 224) == 192)
			    {
				    // If an UCS fits 11 bits, it is coded as 110xxxxx 10xxxxxx
				    if (++i == count) break;
				    byte byte1 = buffer[i];
				    int ch = (byte0 & 31) << 6;
				    ch |= (byte1 & 63);
				    sb.Append((char)ch);
			    }
			    else if ((byte0 & 240) == 224)
			    {
				    // If an UCS fits 16 bits, it is coded as 1110xxxx 10xxxxxx 10xxxxxx
				    if (++i == count) break;
				    byte byte1 = buffer[i];
				    if (++i == count) break;
				    byte byte2 = buffer[i];

				    if (byte0 == 0xEF && byte1 == 0xBB && byte2 == 0xBF)
				    {
					    // Byte Order Mark -- generally the first 3 bytes in a Windows-saved UTF-8 file. Skip it.
				    }
				    else
				    {
					    int ch = (byte0 & 15) << 12;
					    ch |= (byte1 & 63) << 6;
					    ch |= (byte2 & 63);
					    sb.Append((char)ch);
				    }
			    }
			    else if ((byte0 & 248) == 240)
			    {
				    // If an UCS fits 21 bits, it is coded as 11110xxx 10xxxxxx 10xxxxxx 10xxxxxx 
				    if (++i == count) break;
				    byte byte1 = buffer[i];
				    if (++i == count) break;
				    byte byte2 = buffer[i];
				    if (++i == count) break;
				    byte byte3 = buffer[i];

				    int ch = (byte0 & 7) << 18;
				    ch |= (byte1 & 63) << 12;
				    ch |= (byte2 & 63) << 6;
				    ch |= (byte3 & 63);
				    sb.Append((char)ch);
			    }
		    }
		    return sb.ToString();
    #else
            return Encoding.UTF8.GetString(buffer, start, count);
    #endif
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

    #if UNITY_FLASH
			    string[] split = line.Split(separator, System.StringSplitOptions.RemoveEmptyEntries);
    #else
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
    #endif

                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim();
                    if (val.Contains("\\n"))
                    {
                        string[] array = val.Split(new char[] { '\\','n' });
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
                        dict.Add(key,val);
                }
            }
            return dict;
        }
    }
}
