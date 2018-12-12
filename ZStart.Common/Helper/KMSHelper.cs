using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using ZStart.Common.Util;

namespace ZStart.Common.Helper
{
    public class KMSHelper
    {
        public enum KMSState
        {
            Correct = 0,
            ValidatorError = 1,
            FormatError = 2,
            CERError = 3,
            AESDecryptError = 4,
            SigError = 5,
            SignCheckFailed = 6,
            TimeError = 7,
            ExpiryError = 8,
            Expiry = 9
        }

        public KMSHelper()
        {
           
        }

        public KMSState CheckLicense(string source,string sn)
        {
            string app = "com.yumei.zps";
            string keyCode = app + "-key";
            string secretCode = app + "-secret";
            string appKey = ToMD5(keyCode);
            string appSecet = ToMD5(secretCode);
            string codes = ToMD5(sn);
            //Debug.Log("sn = "+sn + ";key = " + keyCode + ";secret = " + secretCode + ";appkey = " + appKey+";appSecret = "+appSecet+ ";codes = "+ codes);
            return VerifyLicense(source, appKey, appSecet, codes);
        }

        private string GetPassword(string appKey, string appSecret) {
            string hash = appKey + "*!@#zps#@!*" + appSecret;
            return ToMD5(hash.Trim()).ToUpper();
        }

        private byte[] Encode(byte[] src) {
            string hextable = "0123456789abcdef";
            char[] array = hextable.ToCharArray();
            byte[] dst = new byte[src.Length*2];
            for (int i = 0;i < src.Length; i++) {
                byte v = src[i];
                dst[i * 2] = (byte)array[v >> 4];
                dst[i * 2 + 1] = (byte)array[v & 0x0f];
            }
            return dst;
        }

        private string ToMD5(string msg)
        {
           MD5 md5 = new MD5CryptoServiceProvider();
           byte[] fromData = Encoding.Default.GetBytes(msg);
           byte[] targetData = md5.ComputeHash(fromData);
           return AlgorithmUtil.ByteToHexStr(targetData);
        }

        private string ToMD5(byte[] source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] targetData = md5.ComputeHash(source);
            return AlgorithmUtil.ByteToHexStr(targetData);
        }

        private void PrintArray(byte[] array)
        {
            string tmp = "";
            for (int i = 0;i < array.Length;i++)
            {
                tmp += array[i] + " ";
            }
            Debug.Log(tmp);
        }

        /// <summary>
        ///  AES 加密
        /// </summary>
        /// <param name="str">明文（待加密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public byte[] AesEncrypt(string str, string key)
        {
            if (string.IsNullOrEmpty(str)) return null;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

            byte[] keys = Encoding.UTF8.GetBytes(key);
            byte[] ivs = new byte[16];
            for (int i = 0;i < 16;i++)
            {
                ivs[i] = keys[i];
            }
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = keys,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = ivs
            };

            ICryptoTransform cTransform = rm.CreateEncryptor();
            byte[] result = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return result;
        }
        /// <summary>
        ///  AES 解密
        /// </summary>
        /// <param name="str">明文（待解密）</param>
        /// <param name="key">密文</param>
        /// <returns></returns>
        public byte[] AesDecrypt(byte[] data, string key)
        {
            if (data == null || data.Length < 1)
                return null;

            byte[] keys = Encoding.UTF8.GetBytes(key);
            byte[] ivs = new byte[16];
            for (int i = 0; i < 16; i++)
            {
                ivs[i] = keys[i];
            }
            RijndaelManaged rm = new RijndaelManaged
            {
                Key = keys,
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                IV = ivs,
            };

            ICryptoTransform cTransform = rm.CreateDecryptor();
            Byte[] result = cTransform.TransformFinalBlock(data, 0, data.Length);

            return result;
        }

        /// <summary>  
        /// RSA的加密函数   
        /// </summary>  
        /// <param name="xmlPublicKey">公钥</param>  
        /// <param name="EncryptString">待加密的字节数组</param>  
        /// <returns></returns>  
        public string RSAEncrypt(string xmlPublicKey, byte[] EncryptString)
        {
            try
            {
                string Result;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPublicKey);
                byte[] array = rsa.Encrypt(EncryptString, false);
                Result = Convert.ToBase64String(array);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>  
        /// RSA的解密函数  
        /// </summary>  
        /// <param name="xmlPrivateKey">私钥</param>  
        /// <param name="decryptString">待解密的字符串</param>  
        /// <returns></returns>  
        public string RSADecrypt(string xmlPrivateKey, string decryptString)
        {
            try
            {
                byte[] PlainTextBArray;
                byte[] DypherTextBArray;
                string Result;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPrivateKey);
                PlainTextBArray = Convert.FromBase64String(decryptString);
                DypherTextBArray = rsa.Decrypt(PlainTextBArray, false);
                Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>  
        /// RSA的解密函数   
        /// </summary>  
        /// <param name="xmlPrivateKey">私钥</param>  
        /// <param name="DecryptString">待解密的字节数组</param>  
        /// <returns></returns>  
        public string RSADecrypt(string xmlPrivateKey, byte[] DecryptString)
        {
            try
            {
                byte[] DypherTextBArray;
                string Result;
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                rsa.FromXmlString(xmlPrivateKey);
                DypherTextBArray = rsa.Decrypt(DecryptString, false);
                Result = (new UnicodeEncoding()).GetString(DypherTextBArray);
                return Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private bool RSAVerify(string publicKey, byte[] data, byte[] sign) {

            try
            {
                string[] array = publicKey.Split('\n');
                if (array == null || array.Length < 3)
                {
                    Debug.LogError("pem format error!!!");
                    return false;
                }
                
                string tmp = "";
                int len = array.Length - 2;
                for (int i = 1;i < len; i++)
                {
                    tmp += array[i];
                }
               
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(tmp));
                string xml = string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent></RSAKeyValue>", 
                    Convert.ToBase64String(publicKeyParam.Modulus.ToByteArrayUnsigned()), 
                    Convert.ToBase64String(publicKeyParam.Exponent.ToByteArrayUnsigned()));
                var sha256 = SHA256.Create();
                byte[] rgbHash = sha256.ComputeHash(data);
                //Debug.LogWarning(tmp);
                //PrintArray(rgbHash);
                //PrintArray(sign);
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider();
                provider.FromXmlString(xml);
                RSAPKCS1SignatureDeformatter deformatter = new RSAPKCS1SignatureDeformatter(provider);
                deformatter.SetHashAlgorithm("SHA256");
                if (deformatter.VerifySignature(rgbHash, sign))
                {
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                Debug.LogWarning(e.Message);
                return false;
            }
        }

        private string FilterString(string msg)
        {
            string dummyData = msg.Trim().Replace("%", "").Replace(",", "").Replace(" ", "+");
            if (dummyData.Length % 4 > 0)
            {
                dummyData = dummyData.PadRight(dummyData.Length + 4 - dummyData.Length % 4, '=');
            }
            return dummyData;
        }

        private KMSState VerifyLicense(string license, string appKey, string appSecret, string deviceCode)
        {
            string[] array = license.Split('\n');
            if (array == null || array.Length < 14)
            {
                return KMSState.ValidatorError;
            }

            if ("key:" != array[0] ||
                "code:" != array[2] ||
                "timestamp:" != array[4] ||
                "expiry:" != array[6] ||
                "storage:" != array[8] ||
                "cer:" != array[10] ||
                "sig:" != array[12])
            {
                return KMSState.FormatError;
            }

            string password = GetPassword(appKey, appSecret);
           
            //take payload
            string payload = "key:\n" + appKey + "\ncode:\n" + deviceCode + "\ntimestamp:\n"+ array[5] + "\nexpiry:\n"+ array[7] + "\nstorage:\n"+ array[9] + "\ncer:\n"+ array[11];
           
            byte[] cipher = AesEncrypt(payload, password);
         
            string payloadMd5 = ToMD5(cipher);
            //Debug.Log("psw = " + password);
            //Debug.Log("payload = " + payload);
            //Debug.Log("payload md5 = " + payloadMd5);
            //take cer
            byte[] cerCipher = Convert.FromBase64String(array[11]);

            if (cerCipher == null)
            {
                return KMSState.CERError;
            }

            byte[] cer = AesDecrypt(cerCipher, password);
            if (cer == null)
            {
                return KMSState.AESDecryptError;
            }

            //take sig
            byte[] sigCipher = Convert.FromBase64String(array[13].Trim());
            if (sigCipher == null)
            {
                return KMSState.SigError;
            }
           
            string cerStr = Encoding.UTF8.GetString(cer);          
            bool success = RSAVerify(cerStr, Encoding.UTF8.GetBytes(payloadMd5), sigCipher);
            if (!success)
            {
                return KMSState.SignCheckFailed;
            }

            long timestamp = 0;
            bool suc = long.TryParse(array[5], out timestamp);
            if (!suc)
            {
                return KMSState.TimeError;
            }

            long expiry = 0;
            bool ss = long.TryParse(array[7], out expiry);
            if (!ss)
            {
                return KMSState.ExpiryError;
            }

            if (expiry != 0)
            {
                long now = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000000;
                if (now - timestamp > expiry * 24 * 60 * 60)
                {
                    return KMSState.Expiry;
                }
            }

            return KMSState.Correct;
        }
    }
}
