using System.Security.Cryptography;
using System.Text;

namespace ZStart.Common.Util
{
    public class AlgorithmUtil
    {

        public static string ByteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("x2");
                }
            }
            return returnStr;
        }

        public static string ToMD5(string msg)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.Default.GetBytes(msg);
            byte[] targetData = md5.ComputeHash(fromData);
            return ByteToHexStr(targetData);
        }

        public static string ToMD5(byte[] bytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] array = md5.ComputeHash(bytes);
            return ByteToHexStr(array);
        }
    }
}
