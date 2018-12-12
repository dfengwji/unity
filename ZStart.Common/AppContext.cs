using UnityEngine;
using ZStart.Core;

namespace ZStart.Common
{
    public class AppContext
    {

        public enum AppPrefEnum
        {
            Cookie,
            SN,
        }

        public static string backPanel = "";

        private static AppContext _instance = null;

        //是否自动登录
        private bool _autoLogin = false;
        //是否保存密码
        private bool _savePassword = false;

        protected AppContext()
        {

        }

        public bool autoLogin
        {
            get { return _autoLogin; }
            set { _autoLogin = value; }
        }

        public bool savePassword
        {
            get { return _savePassword; }
            set { _savePassword = value; }
        }

        public static AppContext Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AppContext();
                return _instance;
            }
        }

        static string GetProjectID(string key)
        {
            return "ZStart." + key;
        }

        public static void WriteString(AppPrefEnum key, string value)
        {
            WriteString(key.ToString(), value);
        }

        public static void WriteString(string key, string value)
        {
            try
            {
                PlayerPrefs.SetString(GetProjectID(key), value);
                PlayerPrefs.Save();
            }
            catch (PlayerPrefsException e)
            {
                ZLog.Warning("can not save to pref!!!" + e.Message);
            }
        }

        public static void WriteFloat(AppPrefEnum key, float value)
        {
            try
            {
                PlayerPrefs.SetFloat(GetProjectID(key.ToString()), value);
                PlayerPrefs.Save();
            }
            catch (PlayerPrefsException e)
            {
                ZLog.Warning("can not save to pref!!!" + e.Message);
            }
        }

        public static void WriteInt(AppPrefEnum key, int value)
        {
            try
            {
                PlayerPrefs.SetInt(GetProjectID(key.ToString()), value);
                PlayerPrefs.Save();
            }
            catch (PlayerPrefsException e)
            {
                ZLog.Warning("can not save to pref!!!" + e.Message);
            }
        }


        public static float ReadFloat(AppPrefEnum key)
        {
            return PlayerPrefs.GetFloat(GetProjectID(key.ToString()), 1f);
        }

        public static string ReadString(AppPrefEnum key)
        {
            return PlayerPrefs.GetString(GetProjectID(key.ToString()), "");
        }

        public static string ReadString(string key)
        {
            return PlayerPrefs.GetString(GetProjectID(key), "");
        }

        public static int ReadInt(AppPrefEnum key)
        {
            return PlayerPrefs.GetInt(GetProjectID(key.ToString()), 1);
        }

        public static void DeleteLocalData(AppPrefEnum key)
        {
            PlayerPrefs.DeleteKey(GetProjectID(key.ToString()));
        }

        public static void ClearLoacalData()
        {
            PlayerPrefs.DeleteAll();
        }

        public static bool HasKey(AppPrefEnum key)
        {
            return PlayerPrefs.HasKey(GetProjectID(key.ToString()));
        }

        public static void OpenWeb(string url)
        {
            Application.OpenURL(url);
        }
    }
}
