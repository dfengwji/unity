using System.Collections.Generic;
using UnityEngine;

namespace ZStart.Core.Util
{
    public class StringUtil
    {
        private StringUtil()
        {

        }

        public static bool CheckChineseChar(char c)
        {
            if ((int)c > 127)
                return true;
            else
                return false;
        }

        public static bool CheckChineseUniChar(char c)
        {
            if (c >= 0x4e00 && c <= 0x9fbb)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 中文占两个字符
        /// </summary>
        /// <param name="mess"></param>
        /// <returns></returns>
        public static int GetCharLength(string mess)
        {
            if (string.IsNullOrEmpty(mess))
                return 0;
            int length = 0;
            char[] array = mess.ToCharArray();
            for (int i = 0; i < array.Length;i++ )
            {
                if (CheckChineseUniChar(array[i]))
                    length += 2;
                else
                    length += 1;
            }
            return length;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mess">id,id,id,...</param>
        /// <returns></returns>
        public static uint[] StringToUints(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(',');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            uint[] array = new uint[leng];
            for (int i = 0; i < leng; i++)
            {
                array[i] = uint.Parse(arr[i].ToString());
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mess">id,id,id,...</param>
        /// <returns></returns>
        public static int[] StringToInts(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(',');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            int[] array = new int[leng];
            for (int i = 0; i < leng; i++)
            {
                array[i] = int.Parse(arr[i]);
            }
            return array;
        }

        private static int[] ParseString(string mess)
        {
            if (string.IsNullOrEmpty(mess))
                return null;
            string[] arr = mess.Split(',');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            int[] array = new int[leng];
            for (int i = 0; i < leng; i++)
            {
                array[i] = int.Parse(arr[i]);
            }
            return array;
        }

        public static float[] StringToFloats(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(',');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            float[] array = new float[leng];
            for (int i = 0; i < leng; i++)
            {
                array[i] = float.Parse(arr[i]);
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mess">id,id,id,...</param>
        /// <returns></returns>
        public static long[] StringToLongs(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(',');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            long[] array = new long[leng];
            for (int i = 0; i < leng; i++)
            {
                array[i] = long.Parse(arr[i].ToString());
            }
            return array;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mess">id,id,id;id,id,id;....</param>
        /// <returns></returns>
        public static List<int[]> StringToIntsList(string mess)
        {

            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(';');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            List<int[]> list = new List<int[]>();
            for (int i = 0; i < leng; i++)
            {
                int[] tarray = ParseString(arr[i]);
                if (tarray != null)
                    list.Add(tarray);
            }
            return list;
        }

        public static List<long[]> StringToLongsList(string mess)
        {

            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(';');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;
            List<long[]> list = new List<long[]>();
            for (int i = 0; i < leng; i++)
            {
                long[] tarray = StringToLongs(arr[i]);
                if (tarray != null)
                    list.Add(tarray);
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mess">key,value;key,value;...</param>
        /// <returns></returns>
        public static Dictionary<int, int> StringToDictionary(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(';');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;

            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < leng; i++)
            {
                string[] temp = arr[i].Split(',');
                int id = int.Parse(temp[0]);
                int rate = int.Parse(temp[1]);
                dic.Add(id, rate);
            }
            return dic;
        }

        public static Dictionary<long, int> String2Dictionary(string mess)
        {
            if (string.IsNullOrEmpty(mess) || mess.Equals("0"))
                return null;
            string[] arr = mess.Split(';');
            if (arr == null || arr.Length == 0)
                return null;
            int leng = arr.Length;
            if (string.IsNullOrEmpty(arr[leng - 1]))
                leng -= 1;

            Dictionary<long, int> dic = new Dictionary<long, int>();
            for (int i = 0; i < leng; i++)
            {
                string[] temp = arr[i].Split(',');
                long id = long.Parse(temp[0]);
                int rate = int.Parse(temp[1]);
                dic.Add(id, rate);
            }
            return dic;
        }

        public static string GetTextWithoutBOM(TextAsset textAsset)
        {
            return textAsset.text;
            //StringReader stringReader = new StringReader(textAsset.text); 
            //stringReader.Read(); // skip BOM 

            //return stringReader.ReadToEnd();
            //System.Xml.XmlReader reader = System.Xml.XmlReader.Create(stringReader);
            //MemoryStream memoryStream = new MemoryStream(textAsset.bytes);
            //StreamReader streamReader = new StreamReader(memoryStream, true);

            //string result = streamReader.ReadToEnd();

            //streamReader.Close();
            //memoryStream.Close();

            //return result;
        }

        /// <summary>
        /// 获取字符串的字节长度 Unicode编码
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static int GetByteLengthUTF8(string str)
        {
            if (str == null)
                return -1;
            return System.Text.Encoding.Unicode.GetBytes(str).Length;
        }

        public static string FormatString<T>(T[] array, string lab)
        {
            if (array == null)
                return lab + " Length = null";
            string mess = lab + " Length = " + array.Length + ":{";
            for (int i = 0; i < array.Length; i++)
            {
                mess += array[i].ToString() + ",";
            }
            return mess + "}";
        }

        public static string FormatString<T>(List<T> list, string lab)
        {
            string mess = lab + " Count = " + list.Count + " ：{";
            for (int i = 0; i < list.Count; i++)
            {
                mess += list[i].ToString() + ",";
            }
            return mess + "}";
        }

        public static string GetTextByPath(string path)
        {
            TextAsset asset = Resources.Load(path) as TextAsset;
            if (asset == null)
            {
                ZLog.Warning("Can not find config file that path = " + path + "!!!!");
                return "";
            }
            return StringUtil.GetTextWithoutBOM(asset);
        }
    }
}
