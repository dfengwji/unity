using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace ZStart.RGraph
{
    public class LogoutTool:MonoBehaviour
    {
        private string logDirectory;
        private StreamWriter writer = null;
        private int hour = -1;
     
        void Start()
        {
          
#if UNITY_EDITOR || UNITY_STANDALONE
            logDirectory = Directory.GetParent(Application.dataPath) + "\\Log\\";
            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }
            CheckWriter();
            //在这里做一个Log的监听
            Application.logMessageReceivedThreaded += HandleLog;
#endif
            
        }

        private void CheckWriter()
        {
            DateTime now = DateTime.Now;
            string tmp = now.Year + "-" + now.Month + "-" + now.Day + "_log.txt";
            string path = Path.Combine(logDirectory, tmp);
            hour = now.Hour;
            if (!File.Exists(path))
            {
                File.CreateText(path);
            }
            try
            {
                if (writer != null)
                {
                    //writer.Dispose();
                    writer.Close();
                    writer = null;
                }
                writer = new StreamWriter(path, true, Encoding.UTF8)
                {
                    AutoFlush = true
                };
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message);
            }
            
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            
            string msg = "";
            if (type == LogType.Error || type == LogType.Exception)
            {
                msg = "[ERROR]";
                msg += logString;
                msg += "\n" + stackTrace;
            }
            else
            {
                msg = logString + "\n\t" + stackTrace;
            }
            if (hour != DateTime.Now.Hour || writer == null)
            {
                CheckWriter();
            }
            if (writer != null)
                writer.WriteLine("[" + DateTime.Now + "]:" + msg);
        }
    }
}
