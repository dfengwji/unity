using UnityEngine;
using ZStart.Common.Enum;
using ZStart.Common.Proxy;
using ZStart.Core;

namespace ZStart.Common.Manager
{
    public class LocalManager
    {
        private static LocalManager _instance;

        public static LocalManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocalManager();
                return _instance;
            }
        }

        private LanguageProxy proxy;
        private LanguageType language = LanguageType.Unknown;
        public LanguageType CurrentLanguage
        {
            get
            {
                return language;
            }
        }

        private Font currentFont;
        public Font UsedFont
        {
            get
            {
                return currentFont;
            }
        }
        private LocalManager()
        {
            proxy = new LanguageProxy();
        }

        private LanguageType GetSystemLanguage()
        {
            SystemLanguage system = Application.systemLanguage;
            ZLog.Warning("LocalManager check system language = " + system);
            if (system == SystemLanguage.Japanese)
                return LanguageType.Japanese;
            else if (system == SystemLanguage.Korean)
                return LanguageType.Korean;
            else if (system == SystemLanguage.Chinese
                || system == SystemLanguage.ChineseSimplified
                || system == SystemLanguage.ChineseTraditional)
                return LanguageType.Chinese;
            else
                return LanguageType.English;
        }

        public void CheckSystemLanguage()
        {
            SwitchLanguage(GetSystemLanguage());
        }

        public void SwitchLanguage(LanguageType type)
        {
            ZLog.Warning("LocalManager SwitchLanguage...from = " + language + " to " + type);
            if (language == type)
                return;
            language = type;
            TextAsset asset = null;
            if(type == LanguageType.Chinese){
                asset = Resources.Load<TextAsset>("xml/chinese");
                //LocalManager.currentFont = Font.CreateDynamicFontFromOSFont("Droid Sans Mono", 14);
            }else if(type == LanguageType.English){
                asset = Resources.Load<TextAsset>("xml/english");
                //LocalManager.currentFont = Font.CreateDynamicFontFromOSFont("Droid Sans Mono", 14);
            }else if(type == LanguageType.Japanese){
                asset = Resources.Load<TextAsset>("xml/japanese");
                proxy.Parse(asset.text);
                currentFont = Font.CreateDynamicFontFromOSFont("Noto Sans JP", 14);
            }
            else if (type == LanguageType.Korean)
            {
                asset = Resources.Load<TextAsset>("xml/korean");
            }
         
            if (asset != null)
            {
                proxy.Parse(asset.text);
                NotifyManager.SendNotify(NotifyType.OnLaguageUpdate,null);
            }
        }

        public static string GetValue(string key)
        {
            return Instance.proxy.GetValue(key);
        }

        public static string GetValue(string key, params object[] args)
        {
            return Instance.proxy.GetValue(key,args);
        }

    }
}
