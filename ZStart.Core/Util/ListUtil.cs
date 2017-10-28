using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZStart.Core.Util
{
    public class ListUtil
    {

        private ListUtil()
        {
        }

        public static bool IsNull<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                return true;
            else
                return false;
        }

        public static bool IsNull<T>(List<T> list)
        {
            if (list == null || list.Count == 0)
                return true;
            else
                return false;
        }

        public static List<T> AddElement<T>(T[] array1, T[] array2)
        {
            return AddElement<T>(array1, array2, false);
        }

        public static List<T> AddElement<T>(T[] array1, T[] array2, bool update)
        {
            List<T> list = new List<T>();
            if (update)
            {
                list.AddRange(ConbineArray(array2, array1));
            }
            else
            {
                list.AddRange(ConbineArray(array1, array2));
            }
            list = ArrayFilter(list.ToArray());
            return list;
        }

        public static T[] ConbineArray<T>(T[] array1, T[] array2)
        {
            if (array1 == null && array2 == null)
                return null;
            List<T> list = new List<T>();
            int i = 0;
            for (i = 0; i < array1.Length; i++)
            {
                list.Add(array1[i]);
            }
            for (i = 0; i < array2.Length; i++)
            {
                list.Add(array2[i]);
            }
            return list.ToArray();
        }

        public static Vector3[] CopyArray(Vector3[] sourceList)
        {
            if (sourceList == null || sourceList.Length == 0)
                return null;
            int length = sourceList.Length;
            Vector3[] destList = new Vector3[length];
            for (int i = 0; i < length; i++)
            {
                Vector3 temp = sourceList[i];
                destList[i] = new Vector3(temp.x, temp.y, temp.z);
            }
            return destList;
        }

        public static T[] DifferentArrays<T>(T[] array1, T[] array2)
        {
            if (array1 == null && array2 == null)
                return null;
            T[] carray = ConbineArray<T>(array1, array2);
            List<T> list = new List<T>();
            for (int i = 0; i < carray.Length; i++)
            {
                if (HasElement(array1, carray[i]) && HasElement(array2, carray[i]))
                {

                }
                else
                {
                    list.Add(carray[i]);
                }
            }
            return list.ToArray();
        }

        public static string GetStringByKey(string key, Hashtable table)
        {
            if (table == null) return null;
            foreach (DictionaryEntry entry in table)
            {
                if (key.Equals(entry.Key))
                {
                    return entry.Value.ToString();
                }
            }
            return null;
        }

        public static ArrayList GetListByKey(string key, Hashtable table)
        {
            if (table == null) return null;
            foreach (DictionaryEntry entry in table)
            {
                if (key.Equals(entry.Key))
                {
                    return entry.Value as ArrayList;
                }
            }
            return null;
        }

        public static List<T> ArrayFilter<T>(T[] array)
        {
            if (array == null || array.Length == 0)
                return null;
            List<T> list = new List<T>();
            for (int i = 0; i < array.Length; i++)
            {
                if (!HasElement(list, array[i]))
                {
                    list.Add(array[i]);
                }
            }
            return list;
        }

        public static bool ArraysEqual<T>(T[] array1, T[] array2)
        {
            if (array1 == null || array2 == null)
                return false;
            if (array1.Length != array2.Length)
                return false;
            for (int i = 0; i < array1.Length; i++)
            {
                if (!HasElement(array2, array1[i]))
                    return false;
            }
            return true;
        }

        public static bool ContainElement<T>(T[] array, long id) where T : ZDataBase
        {
            if (array == null || array.Length < 1)
                return false;
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].ID == id)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool HasElement<T>(T[] array, T element)
        {
            if (array == null || array.Length < 1)
                return false;
            for (int i = 0; i < array.Length; i++)
            {
                if (element.Equals(array[i]))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainElement<T>(List<T> array, long id)where T:ZDataBase
        {
            return ContainElement(array.ToArray(), id);
        }

        public static bool HasElement<T>(List<T> array, T element)
        {
            return HasElement(array.ToArray(), element);
        }

        public static string ArrayToString<T>(T[] array)
        {
            string mess = "";
            if (array != null && array.Length > 0)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (i < array.Length - 1)
                    {
                        mess += array[i].ToString() + ",";
                    }
                    else
                    {
                        mess += array[i].ToString();
                    }
                }
            }
            return mess;
        }

        public static string ArrayToString<T>(List<T> array)
        {
            string mess = "";
            if (array != null && array.Count > 0)
            {
                for (int i = 0; i < array.Count; i++)
                {
                    if (i < array.Count - 1)
                    {
                        mess += array[i].ToString() + ",";
                    }
                    else
                    {
                        mess += array[i].ToString();
                    }
                }
            }
            return mess;
        }

        public static int NormalizeIndex(int index, int length)
        {

            if (index < 0)
                index = length - 1;
            else if (index > length - 1)
                index = 0;
            return index;
        }

        public static T[] GetSubList<T>(List<T> orginList, int count)
        {
            List<T> tempList = new List<T>();
            count = Mathf.Min(orginList.Count, count);
            while (tempList.Count < count)
            {
                int index = UnityEngine.Random.Range(0, orginList.Count);
                if (!tempList.Contains(orginList[index]))
                {
                    tempList.Add(orginList[index]);
                }
            }
            return tempList.ToArray();
        }

        public static long[] SwitchIntToLong(List<int> list)
        {
            if (list == null) return null;
            long[] array = new long[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                array[i] = list[i];
            }
            return array;
        }
    }
}
