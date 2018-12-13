using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using ZStart.Core;

namespace ZStart.Common.Manager
{
    public class FileCacheManager
    {
        private static FileCacheManager mInstance = null;
        public static FileCacheManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new FileCacheManager();
                return mInstance;
            }
        }

        private readonly string logoDirectory;
        private readonly string screenDirectory;
        private readonly string videoDirectory;
        private readonly string adDirectory;
        private readonly string rootDirectory;
        private readonly string adImagesDirectory;
        private readonly string xappDirectory;
        private readonly string newAppPath;
        private readonly string xappWinDirectory;
        private readonly string configFilePath;
        private readonly string certificatePath;
        private readonly string actionsDirectory;
        private readonly string localDirectory;
        private readonly string winImageDirectory;
        private readonly string anyImageDirectory;
        private readonly string imageDirectory;
        private readonly string winExeDirectory;
        private FileCacheManager()
        {
            logoDirectory = Path.Combine(Application.persistentDataPath, "Logos");
            screenDirectory = Path.Combine(Application.persistentDataPath, "Screens");
            configFilePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "config.ini");
            
#if UNITY_ANDROID && !UNITY_EDITOR
            certificatePath = Path.Combine(Application.persistentDataPath, "certificate.pfx");
            rootDirectory = "sdcard/ZPS";
            videoDirectory = "/"+rootDirectory + "/Videos";
            logoDirectory = "/"+rootDirectory + "/Logos";
            screenDirectory = "/"+rootDirectory + "/Screens";
            adDirectory = "/"+rootDirectory + "/ADs";
            xappDirectory = "/" + rootDirectory + "/xapp/Android";
            xappWinDirectory = "/" + rootDirectory + "/xapp/Win32";
            newAppPath = "/" + rootDirectory + "/Version";
            actionsDirectory = "/"+rootDirectory + "/Actions";
            localDirectory = "/"+rootDirectory + "/Locals";
            imageDirectory = "/"+rootDirectory + "/Images/Android";
            anyImageDirectory = "/"+rootDirectory + "/Images/Any";
            winImageDirectory = "/"+rootDirectory + "/Images/Win32";
#else
            certificatePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "certificate.pfx");
            rootDirectory = Directory.GetParent(Application.dataPath).Parent+"\\ZPS";
            videoDirectory = Path.Combine(rootDirectory, "Videos");
            logoDirectory = Path.Combine(rootDirectory, "Logos");
            screenDirectory = Path.Combine(rootDirectory, "Screens");
            adDirectory = Path.Combine(rootDirectory, "ADs");
            xappDirectory = Path.Combine(rootDirectory, "xapp/Android");
            xappWinDirectory = Path.Combine(rootDirectory, "xapp/Win32");
            newAppPath = Path.Combine(rootDirectory, "Version");
            actionsDirectory = Path.Combine(rootDirectory, "Actions");
            localDirectory = Path.Combine(rootDirectory,"Locals");
            imageDirectory = Path.Combine(rootDirectory, "Images/Android");
            winImageDirectory = Path.Combine(rootDirectory, "Images/Win32");
            anyImageDirectory = Path.Combine(rootDirectory, "Images/Any");
            winExeDirectory = Path.Combine(rootDirectory, "EXEs");
#endif
            Debug.Log("certificatePath = " + certificatePath);
        }

        public static string ConfigPath
        {
            get
            {
                return Instance.configFilePath;
            }
        }

        public static string LocalDirectory
        {
            get
            {
                return Instance.localDirectory;
            }
        }

        public static string NewVersion
        {
            get
            {
                return Instance.newAppPath;
            }
        }

        public static string ImagesDirectory
        {
            get
            {
                return Instance.imageDirectory;
            }
        }

        public static string AnyImagesDirectory
        {
            get
            {
                return Instance.anyImageDirectory;
            }
        }

        public static string WinImagesDirectory
        {
            get
            {
                return Instance.winImageDirectory;
            }
        }

        public static string WinEXEDirectory
        {
            get
            {
                return Instance.winExeDirectory;
            }
        }

        public static string XAppWinDirectory
        {
            get
            {
                return Instance.xappWinDirectory;
            }
        }

        public static string RootDirectory
        {
            get
            {
                return Instance.rootDirectory;
            }
        }

        public static string VideoDirectory
        {
            get
            {
                return Instance.videoDirectory;
            }
        }

        public static string XAppDirectory
        {
            get
            {
                return Instance.xappDirectory;
            }
        }

        public static string LogoDirectory
        {
            get
            {
                return Instance.logoDirectory;
            }
        }

        public static string ADDirectory
        {
            get
            {
                return Instance.adDirectory;
            }
        }

        public void Init()
        {
            if (Directory.Exists(videoDirectory) == false)
            {
                Directory.CreateDirectory(videoDirectory);
            }
        }

        public string[] GetAllFiles()
        {
            return Directory.GetFiles(rootDirectory);
        }

        public void ClearFiles(List<string> paths)
        {
            if (paths == null || paths.Count < 1)
                return;
            for (int i = 0; i < paths.Count;i++ )
            {
                ClearFile(paths[i]);
            }
        }

        public void ClearDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                if (Directory.Exists(path) == false)
                    return;
                Directory.Delete(path, true);
            }
            catch (Exception e)
            {
                ZLog.Error("delete flie Error : " + e.Message);
            }
        }

        public void ClearFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                if (File.Exists(path) == false)
                    return;
                File.Delete(path);
            }catch(Exception e){
                ZLog.Error("delete flie Error : " + e.Message);
            }
        }

        public bool WriteFile(string path, byte[] bytes)
        {
            if (bytes == null)
                return false;
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(path))
                    File.Delete(path);
                //File.WriteAllBytes(path, bytes);
                using (FileStream stream = new FileStream(path, FileMode.Create))
                {
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Flush(true);
                    stream.Close();
                    stream.Dispose();
                }
                ZLog.Warning("write file success!!!!" + dir + "---" + path);
                return true;
            }
            catch (Exception e)
            {
                ZLog.Exception(e);
                return false;
            }
        }

        public bool WriteText(string path, string txt)
        {
            try
            {
                string dir = Path.GetDirectoryName(path);
                if (Directory.Exists(dir) == false)
                {
                    Directory.CreateDirectory(dir);
                }
                if (File.Exists(path))
                    File.Delete(path);
                File.WriteAllText(path, txt);
                return true;
            }
            catch (Exception e)
            {
                ZLog.Exception(e);
            }
            return false;
        }

        public Texture2D ReadTexture(string path,Vector2 size)
        {
            try
            {
                if (File.Exists(path) == false)
                    return null;
                byte[] bytes = File.ReadAllBytes(path);
                Texture2D texture = new Texture2D((int)size.x, (int)size.y);
                texture.LoadImage(bytes);
                return texture;
            }
            catch (Exception e)
            {
                ZLog.Exception(e);
                return null;
            }
        }

        public string ReadText(string path)
        {
            try
            {
                if (File.Exists(path) == false)
                    return "";
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                ZLog.Exception(e);
                return "";
            }
        }

        public void ReadTxt(string path)
        {
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                string line = "";
                StringBuilder content = new StringBuilder();
                while ((line = sr.ReadLine()) != null)
                {
                    content.Append(line);
                }
                sr.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ClearActionsCache()
        {
            string[] files = ReadFiles(actionsDirectory);
            if (files == null)
                return;

            for (int i = 0;i < files.Length;i++)
            {
                if(File.Exists(files[i]))
                    File.Delete(files[i]);
            }
        }

        /// <summary>
        /// 读取操作记录
        /// </summary>
        /// <returns></returns>
        public List<string> ReadActionsCache()
        {
            try
            {
                string[] files = ReadFiles(actionsDirectory);
                if (files == null)
                    return null;
                List<string> data = new List<string>(files.Length);
                for (int i = 0;i < files.Length;i++)
                {
                    string tmp = ReadText(files[i]);
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        data.Add(tmp);
                    }
                }
                return data;
            }
            catch (Exception e)
            {
                ZLog.Error("read action cache flie error : " + e.Message);
                return null;
            }
        }

        public string ReadActionsNow()
        {
            DateTime now = DateTime.Now;
            string path = Path.Combine(actionsDirectory, now.Year+"-"+now.Month+"-"+now.Day + "_actions.json");
            return ReadText(path);
        }

        public void WriteActionsCache(string json)
        {
            try
            {
                DateTime now = DateTime.Now;
                if (Directory.Exists(actionsDirectory) == false)
                {
                    Directory.CreateDirectory(actionsDirectory);
                }
                
                string path = Path.Combine(actionsDirectory, now.Year + "-" + now.Month + "-" + now.Day + "_actions.json");
               
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                WriteText(path, json);
                ZLog.Log("WriteActionsCache....");
            }
            catch (Exception e)
            {
                ZLog.Error(e.Message);
            }
        }

        public bool HasCertificate()
        {
            if (File.Exists(certificatePath) == false)
                return false;
            else
                return true;
        }

        /// <summary>
        /// 读取证书
        /// </summary>
        /// <returns></returns>
        public string ReadCertificate()
        {
            try
            {
                if (File.Exists(certificatePath) == false)
                    return "";
                return File.ReadAllText(certificatePath);
            }
            catch (Exception e)
            {
                ZLog.Error("read flie error : " + e.Message);
                return "";
            }
        }

        public void WriteCertificate(string msg)
        {
            if (File.Exists(certificatePath))
                File.Delete(certificatePath);
            WriteText(certificatePath, msg);
            ZLog.Log("WriteCertificate....");
        }

        public void WriteADs(string json)
        {
            string path = Path.Combine(adDirectory, "ads.json");
            WriteText(path, json);
        }

        public string ReadADs()
        {
            try
            {
                string path = Path.Combine(adDirectory, "ads.json");
                if (File.Exists(path) == false)
                    return "";
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                ZLog.Error("read flie error : " + e.Message);
                return "";
            }
        }

        public void WriteMenus(string json)
        {
            string path = Path.Combine(rootDirectory, "menus.json");
            ZLog.Log("WriteMenus..."+path);
            WriteText(path, json);
        }

        public string ReadMenus()
        {
            try
            {
                string path = Path.Combine(rootDirectory, "menus.json");
                if (File.Exists(path) == false)
                    return "";
                return File.ReadAllText(path); 
            }
            catch (Exception e)
            {
                ZLog.Error("read flie error : " + e.Message);
                return "";
            }
        }

        public string ReadTerminals()
        {
            try
            {
                string path = Path.Combine(rootDirectory, "terminals.json");
                if (File.Exists(path) == false)
                    return "";
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                ZLog.Error("read flie error : " + e.Message);
                return "";
            }
        }

        public void WriteTerminals(string json)
        {
            string path = Path.Combine(rootDirectory, "terminals.json");
            ZLog.Log("WriteTerminals..." + path);
            WriteText(path, json);
        }

        public List<string> ReadAll(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                {
                    return null;
                }
                string[] directories = Directory.GetDirectories(path);
                string[] files = Directory.GetFiles(path);
                List<string> all = new List<string>();
                if(directories != null && directories.Length > 0)
                    all.AddRange(directories);
                if(files != null && files.Length > 0)
                    all.AddRange(files);
                return all;
            }
            catch (Exception e)
            {
                ZLog.Error("read flies error : " + e.Message);
                return null;
            }
        }

        public string[] ReadFiles(string path)
        {
            try
            {
                if (Directory.Exists(path) == false)
                {
                    return null;
                }
                return Directory.GetFiles(path);
            }
            catch (Exception e)
            {
                ZLog.Error("read flies error : " + e.Message);
                return null;
            }
        }

        public void WriteTasks(string json)
        {
            string path = Path.Combine(rootDirectory, "tasks.json");
            WriteText(path, json);
        }

        public string ReadTasks()
        {
            try
            {
                string path = Path.Combine(rootDirectory, "tasks.json");
                if (File.Exists(path) == false)
                    return "";
                return File.ReadAllText(path);
            }
            catch (Exception e)
            {
                ZLog.Error("read flie error : " + e.Message);
                return "";
            }
        }

        public string SaveLogo(string appid, string url, byte[] pngData)
        {
            try
            {
                if (Directory.Exists(logoDirectory) == false)
                {
                    Directory.CreateDirectory(logoDirectory);
                }
                string path = Path.Combine(logoDirectory, appid);
              
                File.WriteAllBytes(path, pngData);
                return path;
            }
            catch (Exception e)
            {
                ZLog.Error("save flie error : " + e.Message);
                return "";
            }
        }

        public string SaveScreenshot(string appid, int screenid, string url, byte[] pngData)
        {
            try
            {
                if (Directory.Exists(screenDirectory) == false)
                {
                    Directory.CreateDirectory(screenDirectory);
                }
                string path = Path.Combine(screenDirectory, appid +"-"+ screenid.ToString());
                File.WriteAllBytes(path, pngData);
                return path;
            }
            catch (Exception e)
            {
                ZLog.Error("save flie error : " + e.Message);
                return "";
            }
        }
    }
}
