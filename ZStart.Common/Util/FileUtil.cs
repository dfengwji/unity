using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using UnityEngine;
using ZStart.Core;

namespace ZStart.Common.Util
{
    public class FileUtil
    {
        private FileUtil() { }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_filePathName">Zip包的文件路径名</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(string _filePathName, string _outputPath)
        {
            if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath))
            {
                return false;
            }
            ZipConstants.DefaultCodePage = 0;
            try
            {
                return UnzipFile(File.OpenRead(_filePathName), _outputPath);
            }
            catch (Exception _e)
            {
                Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());
                return false;
            }
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_fileBytes">Zip包字节数组</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(byte[] _fileBytes, string _outputPath)
        {
            if ((null == _fileBytes) || string.IsNullOrEmpty(_outputPath))
            {
                return false;
            }

            bool result = UnzipFile(new MemoryStream(_fileBytes), _outputPath);
           
            return result;
        }

        /// <summary>
        /// 解压Zip包
        /// </summary>
        /// <param name="_inputStream">Zip包输入流</param>
        /// <param name="_outputPath">解压输出路径</param>
        /// <param name="_password">解压密码</param>
        /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
        /// <returns></returns>
        public static bool UnzipFile(Stream _inputStream, string _outputPath)
        {
            if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
            {
                return false;
            }

            // 创建文件目录
            if (!Directory.Exists(_outputPath))
                Directory.CreateDirectory(_outputPath);

            // 解压Zip包
            ZipEntry entry = null;
            using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
            {
                while (null != (entry = zipInputStream.GetNextEntry()))
                {
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;
                   
                    string filePath = Path.Combine(_outputPath, entry.Name);
                    //string fileName = Path.GetFileName(entry.Name);
                    //filePath = Path.Combine(_outputPath, fileName);
                    // 创建文件目录
                    if (entry.IsDirectory)
                    {
                        Directory.CreateDirectory(filePath);
                        continue;
                    }

                    // 写入文件
                    try
                    {
                        using (FileStream fileStream = File.Create(filePath))
                        {
                            byte[] bytes = new byte[1024];
                            while (true)
                            {
                                int count = zipInputStream.Read(bytes, 0, bytes.Length);
                                if (count > 0)
                                    fileStream.Write(bytes, 0, count);
                                else
                                {
                                    
                                    break;
                                }
                            }
                        }
                    }
                    catch (Exception _e)
                    {
                        Debug.LogError("[FileUtil.UnzipFile]: " + _e.ToString());
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="filePath">The ziped file.</param>
        /// <param name="destDirectory">The directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public static void UnZip(string filePath, string destDirectory, string password, bool overWrite)
        {
            if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(destDirectory))
            {
                return;
            }

            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            using (ZipInputStream s = new ZipInputStream(File.OpenRead(filePath)))
            {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    //string directoryName = "";
                    string pathToZip = theEntry.Name;
                   
                    //if (pathToZip != "")
                    //    directoryName = Path.GetDirectoryName(pathToZip) + "/";

                    string fileName = Path.GetFileName(pathToZip);

                    if (fileName != "")
                    {
                        string path = Path.Combine(destDirectory, fileName);
                        if ((File.Exists(path) && overWrite) || (!File.Exists(path)))
                        {
                            using (FileStream streamWriter = File.Create(path))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }

        public static string GetEXEFile(string directory)
        {
            try
            {
                if (Directory.Exists(directory) == false)
                {
                    return "";
                }
                string[] files = Directory.GetFiles(directory, "*.exe");
                if (files != null && files.Length > 0)
                {
                    return files[0];
                }else
                {
                    string[] directores = Directory.GetDirectories(directory);
                    if (directores != null && directores.Length > 0)
                    {
                        for (int i = 0; i < directores.Length; i++)
                        {
                            string path = GetEXEFile(directores[i]);
                            if (!string.IsNullOrEmpty(path))
                            {
                                return path;
                            }
                        }
                    }
                    return "";
                }
            }
            catch (Exception e)
            {
                ZLog.Exception(e);
                return "";
            }
        }

        public static string GetSuffix(string path)
        {
            if (!File.Exists(path))
                return "";
            string[] array = path.Split('.');
            if (array != null && array.Length > 0)
            {
                return array[array.Length - 1];
            }
            return "";
        }

        public static string GetDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return path;
            }
            //if (!File.Exists(path))
            //    return "";
            string[] array = path.Split('.');
            if (array != null && array.Length > 0)
            {
                string tmp = "";
                for (int i = 0;i < array.Length - 1;i++)
                {
                    tmp += array[i];
                }
                return tmp;
            }
            else
            {
                return path;
            }
        }

        public static string GetFileName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return "";
            }
            string[] array = null;
            if (path.Contains("/"))
            {
                array = path.Split('/');
            }
            else
            {
                array = path.Split('\\');
            }
           
            if (array != null && array.Length > 0)
            {
                string nname = array[array.Length - 1];
                string[] arr = nname.Split('.');
                if (arr != null && arr.Length > 1)
                {
                    string format = arr[arr.Length - 1];
                    return nname.Substring(0 ,nname.Length - format.Length - 1);
                }
                else
                {
                    return nname;
                }
            }
            return "";
        }
    }
}
