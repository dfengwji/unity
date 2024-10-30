using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ZStart.Core.Manager
{
    public class ZFileManager
    {
        private static ZFileManager mInstance = null;
        public static ZFileManager Instance
        {
            get
            {
                if (mInstance == null)
                    mInstance = new ZFileManager();
                return mInstance;
            }
        }

        public string[] GetFiles(string dir)
        {
            if (Directory.Exists(dir))
            {
                return null;
            }
            return Directory.GetFiles(dir);
        }

        public void ClearFiles(List<string> paths)
        {
            if (paths == null || paths.Count < 1)
                return;
            for (int i = 0; i < paths.Count; i++)
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
            }
            catch (Exception e)
            {
                ZLog.Error("delete flie Error : " + e.Message);
            }
        }

        public long GetFileSize(string path)
        {
            try
            {
                if (!File.Exists(path))
                    return 0;
                FileInfo info = new FileInfo(path);
                return info.Length;
            }
            catch (Exception e)
            {
                ZLog.Error("get file size Error : " + e.Message);
                return 0;
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
                    stream.Flush();
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

        public Texture2D ReadTexture(string path, Vector2 size)
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

        public string ReadString(string path)
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
                return content.ToString();
            }
            catch (IOException e)
            {
                Console.WriteLine(e.ToString());
                return "";
            }
        }
    }
}
