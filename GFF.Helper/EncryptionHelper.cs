/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：ff285a19-d013-4e69-a5e2-9a640cfe6f51
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：EncryptionHelper
 * 创建时间：2017/1/11 15:41:53
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace GFF.Helper
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class EncryptionHelper
    {
        #region AES
        /// <summary>   
        /// AES对称加密算法类   
        /// </summary> 
        public class AESHelper
        {
            /// <summary>
            /// AES加密
            /// </summary>
            /// <param name="encryptString">待加密的密文</param>
            /// <param name="encryptKey">加密密匙</param>
            /// <returns></returns>
            public static string Encrypt(string encryptString, string encryptKey)
            {
                string returnValue;
                byte[] temp = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
                Rijndael AESProvider = Rijndael.Create();
                try
                {
                    string defaultKey = "3B2hb2oYHpmZrFflfdmSon1x";
                    if (string.IsNullOrEmpty(encryptKey))
                    {
                        encryptKey = defaultKey;
                    }
                    if (encryptKey.Length < 24)
                    {
                        encryptKey = encryptKey + defaultKey.Substring(0, 24 - encryptKey.Length);
                    }
                    if (encryptKey.Length > 24)
                    {
                        encryptKey = encryptKey.Substring(0, 24);
                    }
                    byte[] byteEncryptString = Encoding.UTF8.GetBytes(encryptString);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AESProvider.CreateEncryptor(Encoding.UTF8.GetBytes(encryptKey), temp), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(byteEncryptString, 0, byteEncryptString.Length);
                            cryptoStream.FlushFinalBlock();
                            returnValue = Convert.ToBase64String(memoryStream.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;
            }
            /// <summary>
            ///AES 解密
            /// </summary>
            /// <param name="decryptString">待解密密文</param>
            /// <param name="decryptKey">解密密钥</param>
            /// <returns></returns>
            public static string Decrypt(string decryptString, string decryptKey)
            {
                string returnValue = "";
                byte[] temp = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");
                Rijndael AESProvider = Rijndael.Create();
                try
                {
                    string defaultKey = "3B2hb2oYHpmZrFflfdmSon1x";
                    if (string.IsNullOrEmpty(decryptKey))
                    {
                        decryptKey = defaultKey;
                    }
                    if (decryptKey.Length < 24)
                    {
                        decryptKey = decryptKey + defaultKey.Substring(0, 24 - decryptKey.Length);
                    }
                    if (decryptKey.Length > 24)
                    {
                        decryptKey = decryptKey.Substring(0, 24);
                    }
                    byte[] byteDecryptString = Convert.FromBase64String(decryptString);
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, AESProvider.CreateDecryptor(Encoding.UTF8.GetBytes(decryptKey), temp), CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(byteDecryptString, 0, byteDecryptString.Length);
                            cryptoStream.FlushFinalBlock();
                            returnValue = Encoding.UTF8.GetString(memoryStream.ToArray());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;
            }
        }
        #endregion

        #region base64
        /// <summary>
        /// Base64加密、解密
        /// </summary>
        public static class Base64Helper
        {
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="code"></param>
            /// <returns></returns>
            public static string Encrypt(string code)
            {
                return Base64Helper.Encrypt(Encoding.UTF8.GetBytes(code));
            }

            /// <summary>
            /// 加密到Base64String
            /// </summary>
            /// <param name="datas"></param>
            /// <returns></returns>
            public static string Encrypt(byte[] datas)
            {
                string encode = "";
                try
                {
                    encode = Convert.ToBase64String(datas);
                }
                catch
                {
                    encode = "";
                }
                return encode;
            }

            /// <summary>
            /// 解密
            /// </summary>
            /// <param name="code"></param>
            /// <returns></returns>
            public static string Decrypt(string code)
            {
                string decode = "";
                try
                {
                    decode = Encoding.UTF8.GetString(Base64Helper.DecryptForBytes(code));
                }
                catch
                {

                }
                return decode;
            }

            public static byte[] DecryptForBytes(string code)
            {
                return Convert.FromBase64String(code);
            }
        }
        #endregion

        #region DES
        /// <summary>
        /// 数据加密算法
        /// </summary>
        public static class DESHelper
        {
            /// <summary>
            /// DES加密
            /// </summary>
            /// <param name="encryptString">待加密的密文</param>
            /// <param name="encryptKey">密匙（8位）</param>
            /// <returns></returns>
            public static string Encrypt(string encryptString, string encryptKey)
            {
                string returnValue;
                try
                {
                    string defaultKey = "3y6wmlLn3e";
                    if (string.IsNullOrEmpty(encryptKey))
                    {
                        encryptKey = defaultKey;
                    }
                    if (encryptKey.Length < 8)
                    {
                        encryptKey = encryptKey + defaultKey.Substring(0, 8 - encryptKey.Length);
                    }
                    if (encryptKey.Length > 8)
                    {
                        encryptKey = encryptKey.Substring(0, 8);
                    }
                    byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                    DESCryptoServiceProvider dES = new DESCryptoServiceProvider();
                    byte[] byteEncrypt = Encoding.UTF8.GetBytes(encryptString);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, dES.CreateEncryptor(Encoding.UTF8.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                    cryptoStream.Write(byteEncrypt, 0, byteEncrypt.Length);
                    cryptoStream.FlushFinalBlock();
                    returnValue = Convert.ToBase64String(memoryStream.ToArray());

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;
            }
            /// <summary>
            /// DES解密
            /// </summary>
            /// <param name="decryptString">密文</param>
            /// <param name="decryptKey">密匙（8位）</param>
            /// <returns></returns>
            public static string Decrypt(string decryptString, string decryptKey)
            {
                string returnValue;
                try
                {
                    string defaultKey = "3y6wmlLn3e";
                    if (string.IsNullOrEmpty(decryptKey))
                    {
                        decryptKey = defaultKey;
                    }
                    if (decryptKey.Length < 8)
                    {
                        decryptKey = decryptKey + defaultKey.Substring(0, 8 - decryptKey.Length);
                    }
                    if (decryptKey.Length > 8)
                    {
                        decryptKey = decryptKey.Substring(0, 8);
                    }
                    byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                    DESCryptoServiceProvider dES = new DESCryptoServiceProvider();
                    byte[] byteDecryptString = Convert.FromBase64String(decryptString);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, dES.CreateDecryptor(Encoding.UTF8.GetBytes(decryptKey), temp), CryptoStreamMode.Write);

                    cryptoStream.Write(byteDecryptString, 0, byteDecryptString.Length);

                    cryptoStream.FlushFinalBlock();

                    returnValue = Encoding.UTF8.GetString(memoryStream.ToArray());

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;

            }

        }
        #endregion

        #region DES3
        /// <summary>
        /// DES3重安全加密算法处理类
        /// </summary>
        public static class DES3Helper
        {
            /// <summary>
            /// 3DES 加密
            /// </summary>
            /// <param name="encryptString">待加密密文</param>
            /// <param name="encryptKey1">密匙1(长度必须为8位)</param>
            /// <param name="encryptKey2">密匙2(长度必须为8位)</param>
            /// <param name="encryptKey3">密匙3(长度必须为8位)</param>
            /// <returns></returns>
            public static string Encrypt(string encryptString, string encryptKey1, string encryptKey2, string encryptKey3)
            {

                string returnValue;
                try
                {
                    returnValue = DESHelper.Encrypt(encryptString, encryptKey3);
                    returnValue = DESHelper.Encrypt(returnValue, encryptKey2);
                    returnValue = DESHelper.Encrypt(returnValue, encryptKey1);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;

            }
            /// <summary>
            /// 3DES 解密
            /// </summary>
            /// <param name="decryptString">待解密密文</param>
            /// <param name="decryptKey1">密匙1(长度必须为8位)</param>
            /// <param name="decryptKey2">密匙2(长度必须为8位)</param>
            /// <param name="decryptKey3">密匙3(长度必须为8位)</param>
            /// <returns></returns>
            public static string Decrypt(string decryptString, string decryptKey1, string decryptKey2, string decryptKey3)
            {

                string returnValue;
                try
                {
                    returnValue = DESHelper.Decrypt(decryptString, decryptKey1);
                    returnValue = DESHelper.Decrypt(returnValue, decryptKey2);
                    returnValue = DESHelper.Decrypt(returnValue, decryptKey3);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;
            }


        }
        #endregion

        #region MD5加密处理类
        /// <summary>
        /// MD5加密处理类
        /// </summary>
        public static class MD5Helper
        {
            /// <summary>
            /// 字符串MD5加密
            /// </summary>
            /// <param name="inputedPassword"></param>
            /// <returns></returns>
            public static string Encryt(string inputedPassword)
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(inputedPassword, "MD5");
            }
            /// <summary>
            /// 密码比较
            /// </summary>
            /// <param name="inputedPassword"></param>
            /// <param name="currentPassword"></param>
            /// <returns></returns>
            public static bool Verify(string inputedPassword, string currentPassword)
            {
                string encryptedPassword = Encryt(inputedPassword);
                return (encryptedPassword == currentPassword.ToUpper()) ? true : false;
            }
            /// <summary>
            /// 获取文件md5code
            /// </summary>
            /// <param name="stream"></param>
            /// <returns></returns>
            public static string GetHash(Stream stream)
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(stream);
                    var str = System.BitConverter.ToString(retVal);
                    return str.Replace("-", "");
                }
            }
            /// <summary>
            /// 获取文件md5code
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public static string GetHash(byte[] data)
            {
                using (MD5 md5 = new MD5CryptoServiceProvider())
                {
                    byte[] retVal = md5.ComputeHash(data);
                    var str = System.BitConverter.ToString(retVal);
                    return str.Replace("-", "");
                }
            }

            public static string GetFileMd5(string fileName)
            {
                string hashStr = string.Empty;
                try
                {
                    FileStream fs = new FileStream(
                        fileName,
                        FileMode.Open,
                        FileAccess.Read,
                        FileShare.Read);
                    MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                    byte[] hash = md5.ComputeHash(fs);
                    hashStr = ByteArrayToHexString(hash);
                    fs.Close();
                    fs.Dispose();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return hashStr;
            }

            public static string GetStreamMd5(Stream stream)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(stream);
                return ByteArrayToHexString(hash);
            }

            public static string GetDataMd5(byte[] buffer, int offset, int count)
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] hash = md5.ComputeHash(buffer, offset, count);
                return ByteArrayToHexString(hash);
            }

            private static string ByteArrayToHexString(byte[] values)
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte value in values)
                {
                    sb.AppendFormat("{0:X2}", value);
                }
                return sb.ToString();
            }
        }
        #endregion

        #region RC2加密处理类
        /// <summary>
        /// RC2加密处理类
        /// </summary>
        public static class RC2Helper
        {
            /// <summary>
            /// RC2加密
            /// </summary>
            /// <param name="encryptString">待加密的密文</param>
            /// <param name="encryptKey">密匙(必须为5-16位)</param>
            /// <returns></returns>
            public static string Encrypt(string encryptString, string encryptKey)
            {
                string returnValue;
                try
                {
                    byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                    RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
                    byte[] byteEncryptString = Encoding.UTF8.GetBytes(encryptString);
                    MemoryStream memorystream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memorystream, rC2.CreateEncryptor(Encoding.UTF8.GetBytes(encryptKey), temp), CryptoStreamMode.Write);
                    cryptoStream.Write(byteEncryptString, 0, byteEncryptString.Length);
                    cryptoStream.FlushFinalBlock();
                    returnValue = Convert.ToBase64String(memorystream.ToArray());

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;

            }
            /// <summary>
            /// RC2解密
            /// </summary>
            /// <param name="decryptString">密文</param>
            /// <param name="decryptKey">密匙(必须为5-16位)</param>
            /// <returns></returns>
            public static string Decrypt(string decryptString, string decryptKey)
            {
                string returnValue;
                try
                {
                    byte[] temp = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
                    RC2CryptoServiceProvider rC2 = new RC2CryptoServiceProvider();
                    byte[] byteDecrytString = Convert.FromBase64String(decryptString);
                    MemoryStream memoryStream = new MemoryStream();
                    CryptoStream cryptoStream = new CryptoStream(memoryStream, rC2.CreateDecryptor(Encoding.UTF8.GetBytes(decryptKey), temp), CryptoStreamMode.Write);
                    cryptoStream.Write(byteDecrytString, 0, byteDecrytString.Length);
                    cryptoStream.FlushFinalBlock();
                    returnValue = Encoding.UTF8.GetString(memoryStream.ToArray());
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                return returnValue;
            }

        }
        #endregion

        #region SHA1加密处理类
        /// <summary>
        /// SHA1加密处理类
        /// </summary>
        public static class SHA1Helper
        {
            /// <summary>
            /// 字符串SHA1加密
            /// </summary>
            /// <param name="inputedPassword"></param>
            /// <returns></returns>
            public static string Encryt(string inputedPassword)
            {
                return FormsAuthentication.HashPasswordForStoringInConfigFile(inputedPassword, "SHA1");
            }
            /// <summary>
            /// 密码比较
            /// </summary>
            /// <param name="inputedPassword"></param>
            /// <param name="currentPassword"></param>
            /// <returns></returns>
            public static bool Verify(string inputedPassword, string currentPassword)
            {
                string encryptedPassword = Encryt(inputedPassword);
                return (encryptedPassword == currentPassword.ToUpper()) ? true : false;
            }
        }
        #endregion
    }
}
