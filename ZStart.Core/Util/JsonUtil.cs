using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ZStart.Core.Util
{
    public class JsonUtil
    {
        private JsonUtil() { }

        public static string FormatJson(string key, string val)
        {
            return "\"" + key + "\"" + ":" + "\"" + val + "\"";
        }

        public static string FormatJson(string key, long[] array)
        {
            string msg = "\"" + key + "\":[";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < (array.Length - 1))
                        msg += array[i] + ",";
                    else
                        msg += array[i];
                }
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson(string key, int[] array)
        {
            string msg = "\"" + key + "\":[";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < (array.Length - 1))
                        msg += array[i] + ",";
                    else
                        msg += array[i];
                }
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson(string key, byte[] array)
        {
            string msg = "\"" + key + "\":[";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < (array.Length - 1))
                        msg += array[i] + ",";
                    else
                        msg += array[i];
                }
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson(string key, string[] array)
        {
            string msg = "\"" + key + "\":[";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < (array.Length - 1))
                        msg += "\"" + array[i] + "\",";
                    else
                        msg += "\"" + array[i] + "\"";
                }
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson(string key, object[] array)
        {
            string msg = "\"" + key + "\":[";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < (array.Length - 1))
                        msg += array[i].ToString() + ",";
                    else
                        msg += array[i].ToString();
                }
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson(string key, float val)
        {
            return "\"" + key + "\":" + Math.Round(val, 4).ToString();
        }

        public static string FormatJson(string key, object val)
        {
            if(val == null)
                return "\"" + key + "\":null";
            if(val is string)
                return "\"" + key + "\":\"" + val.ToString()+"\"";
            else
                return "\"" + key + "\":" + val.ToString();
        }

        public static string FormatJson(string key, int val)
        {
            return "\"" + key + "\":" + val.ToString();
        }

        public static string FormatJson(string key, Vector3 v3)
        {
            return "\"" + key + "\":{" + FormatJson("x", v3.x) + "," + FormatJson("y", v3.y) + "," + FormatJson("z", v3.z) + "}";
        }

        public static string FormatJson(Vector3 v3)
        {
            return "{" + FormatJson("x", v3.x) + "," + FormatJson("y", v3.y) + "," + FormatJson("z", v3.z) + "}";
        }

        public static string FormatJson(string key, Color color)
        {
            return "\"" + key + "\":{" + FormatJson("r", color.r) + ","
                + FormatJson("g", color.g) + "," + FormatJson("b", color.b)
                + "," + FormatJson("a", color.a) + "}";
        }

        public static string FormatJson(string key, List<Vector3> list)
        {
            string msg = "\"" + key + "\":[";
            foreach (Vector3 v3 in list)
            {
                msg += FormatJson(v3) + ",";
            }
            msg += "]";
            return msg;
        }

        public static string FormatJson<T0,T1>(string key,string keyName,string valName, Dictionary<T0, T1> dic)
        {
            return "\"" + key + "\":"+ FormatJson(keyName,valName,dic);
        }

        public static object FormatJson<T0,T1>(string keyName, string valName, Dictionary<T0, T1> dic)
        {
            StringBuilder msg = new StringBuilder();
            if (dic == null)
                return msg.Append("[]");
            int index = 0;
            msg.Append("[");
            foreach (KeyValuePair<T0, T1> pair in dic)
            {
                if (index < dic.Count - 1)
                {
                    msg.Append("{" + FormatJson(keyName, pair.Key) + ",");
                    msg.Append(FormatJson(valName, pair.Value) + "},");
                }
                else
                {
                    msg.Append("{" + FormatJson(keyName, pair.Key) + ",");
                    msg.Append(FormatJson(valName, pair.Value) + "}");
                }
                index++;
            }
            msg.Append("]");
            return msg;
        }
    }
}
