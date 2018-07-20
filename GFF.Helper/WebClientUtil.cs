/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：20da4241-0bdc-4a06-8793-6d0889c31f95
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：MCITest


 * 创建年份：2015
 * 创建时间：2015-12-02 11:15:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.IO;
using System.Net;
using System.Text;

namespace GFF.Helper
{
    /// <summary>
    ///     自定义webclient
    /// </summary>
    public class WebClientUtil : WebClient
    {
        /// <summary>
        ///     重写后支持自解压
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        protected override WebRequest GetWebRequest(Uri address)
        {
            var request = base.GetWebRequest(address) as HttpWebRequest;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip |
                                             DecompressionMethods.None;
            return request;
        }

        /// <summary>
        ///     将实体发送给远程服务器
        ///     发送json
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string Post(string url, object obj)
        {
            return Post(url, SerializeHelper.Serialize(obj));
        }

        /// <summary>
        ///     发送实体到服务器并返回实体
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="url"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static T2 Post<T1, T2>(string url, T1 t)
        {
            var json = Post(url, SerializeHelper.Serialize(t));
            if (string.IsNullOrEmpty(json))
                return default(T2);
            return SerializeHelper.Deserialize<T2>(json);
        }

        /// <summary>
        ///     将json发送给远程服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static string Post(string url, string json, WebHeaderCollection headers = null)
        {
            using (var client = new WebClientUtil())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.Accept, "*/*");
                client.Headers.Add(HttpRequestHeader.UserAgent,
                    "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                client.Headers.Add("Accept-Encoding", "gzip");
                client.Headers.Add("ContentEncoding", "gzip");
                client.Headers.Add("Content-Type", "application/json; charset=UTF-8");
                if (headers != null)
                    foreach (var item in headers.AllKeys)
                        client.Headers.Add(item, headers[item]);
                return client.UploadString(url, "POST", json);
            }
        }

        /// <summary>
        ///     上传数据到服务器
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static byte[] Post(string url, byte[] data, WebHeaderCollection headers = null)
        {
            using (var client = new WebClientUtil())
            {
                client.Encoding = Encoding.UTF8;
                client.Headers.Add(HttpRequestHeader.Accept, "*/*");
                client.Headers.Add(HttpRequestHeader.UserAgent,
                    "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)");
                client.Headers.Add("Accept-Encoding", "gzip");
                client.Headers.Add("ContentEncoding", "gzip");
                if (headers != null)
                    foreach (var item in headers.AllKeys)
                        client.Headers.Add(item, headers[item]);
                return client.UploadData(url, "POST", data);
            }
        }

        public static bool IsFileInUse(string fileName)
        {
            var inUse = true;

            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,
                    FileShare.None);

                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse; //true表示正在使用,false没有使用  
        }


        public static bool SaveFile(string url, string fileName)
        {
            var result = false;
            try
            {
                using (var webClient = new WebClientUtil())
                {
                    webClient.DownloadFile(url, fileName);
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }
    }
}