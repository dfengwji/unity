using System;
using System.IO;
using System.Text;
using ZStart.Core.ZLib;
using ZStart.Core.Util;
using ZStart.Core.Zlib.GZip;

namespace ZStart.Core.Zlib
{
    public class ZIPUtil
    {
        /// <returns>The string in compressed form</returns>
        public static byte[] CompressString(string mess)
        {
            MemoryStream stream = new MemoryStream();
            GZipOutputStream gzip = new GZipOutputStream(stream);
            byte[] binary = Encoding.UTF8.GetBytes(mess);
            gzip.Write(binary, 0, binary.Length);
            gzip.Close();
            byte[] press = stream.ToArray();
            return press;
        }


        public static string UncompressString(byte[] compressed)
        {
            GZipInputStream gzip = new GZipInputStream(new MemoryStream(compressed));
            MemoryStream stream = new MemoryStream();
            int count = 0;
            byte[] data = new byte[4096];
            while ((count = gzip.Read(data, 0, data.Length)) != 0)
            {
                stream.Write(data, 0, count);
            }
            byte[] depress = stream.ToArray();
            return Encoding.UTF8.GetString(depress);
        }
    }
}
