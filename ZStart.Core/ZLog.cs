using System;
using UnityEngine;

namespace ZStart.Core
{
    public class ZLog
    {
        private static string prefix = "";
        public static bool isLog = false;

        public static string Prefix
        {
            set
            {
                prefix = value;
            }
        }

        private static string GetPrefix()
        {
            var f = "---  [ZStart]: ";
            if (!string.IsNullOrEmpty(prefix))
            {
                f = "-- [ " + prefix + "]:";
            }
            return f;
        }

        public static void Log(object message)
        {
            var f = GetPrefix();
            if(isLog)
                Debug.Log(DateTime.Now.ToString()+f+message);
        }

        public static void Warning(object message)
        {
            var f = GetPrefix();
            if (isLog)
                Debug.LogWarning(DateTime.Now.ToString() + f + message);
        }

        public static void Exception(Exception exception)
        {
            if (isLog)
                Debug.LogException(exception);
        }

        public static void Error(object message)
        {
            var f = GetPrefix();
            if (isLog)
                Debug.LogError(DateTime.Now.ToString() + f + message);
        }
    }
}
