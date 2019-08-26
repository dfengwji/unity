using ZStart.Common.Enum;
using ZStart.Common.Model;
using ZStart.Core.Common;
using ZStart.Core.Enum;
using ZStart.Core.Model;
using System.Collections.Generic;
using UnityEngine;
namespace ZStart.Common.Manager
{
    public class ConfigManager
    {
        private static ConfigManager mInstance = null;
        public static ConfigManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new ConfigManager();
                return mInstance;
            }
        }

        private List<AssetInfo> audioList = null;
        private List<AssetInfo> prefabAssets = null;
      
        private List<ConfigFileInfo> _secondFiles;
        private List<ConfigFileInfo> _firstFiles;
        private XMLParseHelper parser;

        private ConfigManager()
        {
            parser = new XMLParseHelper();
            _secondFiles = new List<ConfigFileInfo>();
            _firstFiles = new List<ConfigFileInfo>();
         
        }

        public List<AssetInfo> GetAssets(AssetType type)
        {
            List<AssetInfo> list = new List<AssetInfo>();
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (prefabAssets[i].type == type)
                    list.Add(prefabAssets[i]);
            }
            return list;
        }

        public List<AssetInfo> AllAssets
        {
            get
            {
                return prefabAssets;
            }
        }

        public AssetInfo GetAssetInfo(string uname)
        {
            for (int i = 0; i < prefabAssets.Count; i++)
            {
                if (prefabAssets[i].name == uname)
                    return prefabAssets[i];
            }
            return new AssetInfo();
        }

        public void AddFile(ConfigFileInfo info,bool first)
        {
            if(first){
                if (HasSameFile(info.name,true) == false)
                    _firstFiles.Add(info);
            }else{
                if (HasSameFile(info.name, false) == false)
                    _secondFiles.Add(info);
            }
        }

        public void AddFile(string path, ConfigFileType type, string text,bool first,long parent)
        {
            ConfigFileInfo file = new ConfigFileInfo();
            file.type = type;
            string[] arr = path.Split('/');
            file.name = arr[arr.Length - 1];
            file.fullPath = "Assets/Resources/Config/" + path + ".xml";
            file.text = text;
            file.parent = parent;
            AddFile(file, first);
        }

        public void AddFirstFile(string path, ConfigFileType type, string text)
        {
            AddFile(path,type,text,true,0);
        }

        public void AddFirstFile(string path, ConfigFileType type, string text,long parent)
        {
            AddFile(path, type, text, true, parent);
        }

        public void AddSecondFile(string path, ConfigFileType type, long parent)
        {
            AddFile(path,type,"",false,parent);
        }

        public void AddSecondFile(string path, ConfigFileType type)
        {
            AddFile(path, type, "", false, 0);
        }

        public void AddFile(ConfigFileInfo info)
        {
            AddFile(info,true);
        }

        public bool HasSameFile(string path, bool first)
        {
            List<ConfigFileInfo> list = null;
            if (first)
            {
                list = _firstFiles;
            }
            else
            {
                list = _secondFiles;
            }
            for (int i = 0; i < list.Count; i++)
            {
                ConfigFileInfo info = list[i];
                if (info.name == path)
                    return true;
            }
            return false;
        }

        public ConfigFileInfo GetFileByPath(string path,bool first)
        {
            List<ConfigFileInfo> list = null;
            if(first){
                list = _firstFiles;
            }else{
                list = _secondFiles;
            }
            for (int i = 0; i < list.Count;i++ )
            {
                ConfigFileInfo info = list[i];
                if (info.name == path)
                    return info;
            }
            return new ConfigFileInfo();
        }

        public void UpdateFileFromBundle(ref AssetBundle bundle)
        {
            if(bundle == null)
                return;
            for (int i = 0; i < _firstFiles.Count;i++ )
            {
                ConfigFileInfo info = _firstFiles[i];
                string uname = info.fullPath.ToLower();
            
                if (bundle.Contains(uname))
                {
                    TextAsset asset = bundle.LoadAsset<TextAsset>(uname);
                    if (asset != null)
                    {
                        info.text = asset.text;
                    }
                }
            }
       
            for (int i = 0; i < _secondFiles.Count; i++)
            {
                ConfigFileInfo info2 = _secondFiles[i];
                string uname = info2.fullPath.ToLower();
                if (bundle.Contains(uname))
                {
                    TextAsset asset = bundle.LoadAsset<TextAsset>(uname);
                    if (asset != null)
                    {
                        info2.text = asset.text;  
                    }
                }
            }
        }

        public void UpdateFileByPath(string path,string text,bool first)
        {
            ConfigFileInfo info = GetFileByPath(path,first);
            if (info.fullPath != path)
            {
                info = new ConfigFileInfo();
                info.name = path;
                info.fullPath = path;
                info.text = text;
                AddFile(info, first);
            }
            else
            {
                info.text = text;
            }
        }

        public void ParseAllFile()
        {
            for (int i = 0; i < _firstFiles.Count; i++)
            {
                ConfigFileInfo info1 = _firstFiles[i];
                ParseFile(info1.parent,info1.type, info1.text);
            }
            for (int i = 0; i < _secondFiles.Count;i++ )
            {
                ConfigFileInfo info2 = _secondFiles[i];
                ParseFile(info2.parent,info2.type, info2.text);
            }
        }


        private void ParseFile(long parent,ConfigFileType type,string text)
        {
            switch(type){
                case ConfigFileType.Audio:
                    audioList = parser.ParseAudioConfig(text);
                    break;
           
                case ConfigFileType.UI:
                    prefabAssets = parser.ParseFixAssetsConfig(text);
                    break;
            }
        }

        public class XMLParseHelper
        {
            public List<AssetInfo> ParseFixAssetsConfig(string xml)
            {
                if (string.IsNullOrEmpty(xml))
                    return null;
                XMLReader reader = new XMLReader(xml);
                List<AssetInfo> infoList = new List<AssetInfo>();
                List<XMLNode> list = reader.GetNodeList("item");
                foreach (XMLNode node in list)
                {
                    AssetInfo model = new AssetInfo
                    {
                        name = node.GetAttribute("name"),
                        type = (AssetType)int.Parse(node.GetAttribute("type")),
                        bundle = uint.Parse(node.GetAttribute("bundle")),
                        preInstant = int.Parse(node.GetAttribute("instant")),
                        asset = node.GetAttribute("path"),
                        audio = uint.Parse(node.GetAttribute("music")),
                        grade = int.Parse(node.GetAttribute("grade"))
                    };
                    infoList.Add(model);
                }
                return infoList;
            }

            public List<AssetInfo> ParseAudioConfig(string text)
            {
                if (string.IsNullOrEmpty(text)) return null;
                XMLReader reader = new XMLReader(text);
               // List<XMLNode> list = reader.GetNodeList("audio");

                List<AssetInfo> models = new List<AssetInfo>();
                //foreach (XMLNode node in list)
                //{
                //    AssetInfo info = new AssetInfo();
                //    info.type = AssetType.AudioClip;
                //    info. = int.Parse(node.GetAttribute("id"));
                //    info.loop = int.Parse(node.GetAttribute("loop")) == 1 ? true : false;
                //    info.path = node.GetAttribute("path");
                //    info.time = float.Parse(node.GetAttribute("time"));
                //    info.bundle = int.Parse(node.GetAttribute("bundle"));
                //    string[] array = info.path.Split('/');
                //    if (array != null && array.Length > 0)
                //    {
                //        info.name = array[array.Length - 1];
                //    }
                //    models.Add(info);
                //}
                return models;
            }
        }
    }
}
