using System;
using UnityEngine;

namespace ZStart.Core
{
    public class ZLog
    {
        public static bool isLog = false;

        public static void Log(object message)
        {
            if(isLog)
                Debug.Log(DateTime.Now.ToString()+"-- ZStart: "+message);
        }

        public static void Warning(object message)
        {
            if (isLog)
                Debug.LogWarning(DateTime.Now.ToString() + "-- ZStart: " + message);
        }

        public static void Exception(Exception exception)
        {
            if (isLog)
                Debug.LogException(exception);
        }

        public static void Error(object message)
        {
            if (isLog)
                Debug.LogError(DateTime.Now.ToString() + "-- ZStart: " + message);
        }
    }
}
