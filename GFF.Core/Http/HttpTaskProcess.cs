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
using System.Threading.Tasks;
using System.Web;

namespace GFF.Core.Http
{
    /// <summary>
    ///     http服务任务处理类
    ///     此类需要以管理员启动
    ///     503异常
    ///     开始运行“cmd”
    ///     输入“netsh”回车
    ///     输入“http”回车
    ///     输入“show urlacl”回车（这时你已经能看见你设定的端口号的一些URL地址，如：http://+:9997/）
    ///     输入“delete urlacl http://+:9997/”
    /// </summary>
    public static class HttpTaskProcess
    {
        private static readonly string path = Environment.CurrentDirectory + "\\http\\";

        /// <summary>
        ///     处理客户端的各种请求
        /// </summary>
        /// <param name="obj"></param>
        public static void Process(object obj)
        {
            if (obj != null)
            {
                var current = obj as HttpListenerContext;
                var request = current.Request;
                using (var response = current.Response)
                {
                    try
                    {                        
                        response.Headers.Add(HttpResponseHeader.Server, "GFF-PowerBy wenli");
                        if (request.HttpMethod == "GET")
                        {
                            if (request.Url.ToString().IndexOf("favicon.ico") > -1)
                            {
                                ResponseFile(response, path + "favicon.ico");
                                return;
                            }
                            //下载文件
                            var surl = request.Url.Query.ToLower();
                            if (surl.IndexOf("?filename=") > -1)
                            {
                                var fileName = HttpUtility.UrlDecode(surl.Substring(surl.LastIndexOf("=") + 1));
                                if (!string.IsNullOrWhiteSpace(fileName))
                                {
                                    var filePath = path + fileName.ToLower();

                                    if (File.Exists(filePath))
                                    {
                                        ResponseFile(response, filePath);

                                        return;
                                    }
                                }
                            }
                            ResponseHtml(response, "卧擦404了，找不到要展示的内容！");
                            return;
                        }
                        if (request.HttpMethod == "POST")
                        {
                            //上传文件
                            //获取Post请求中的参数和值帮助类  
                            using (var httppost = new HttpPostValueHelper(request))
                            {
                                //获取Post过来的参数和数据  
                                //接收多个
                                var list = httppost.GetHttpPostItemList();
                                //当前只处理一个，可改为多处理
                                Parallel.ForEach(list, item =>
                                {
                                    if (item.type == 1)
                                    {
                                        var filePath = path + item.name;
                                        if (!File.Exists(filePath))
                                            try
                                            {
                                                using (var fs = new FileStream(filePath, FileMode.Create))
                                                {
                                                    fs.Write(item.datas, 0, item.datas.Length);
                                                    fs.Flush();
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        ResponseTxt(response,
                                            request.Url + "?fileName=" + HttpUtility.UrlEncode(item.name, Encoding.UTF8));
                                    }
                                });
                                list = null;
                            }
                        }
                        else
                        {
                            ResponseHtml(response, "卧擦404了，找不到要展示的内容！");
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        ResponseHtml(response, "卧擦500了:！" + ex.Message);
                        return;
                    }
                    obj = null;
                }
            }
        }

        private static void ResponseTxt(HttpListenerResponse response, string txt, string extention = ".txt",
            int statusCode = 200)
        {
            try
            {
                var buffer = Encoding.UTF8.GetBytes(txt);
                response.ContentType = MIMETypeProcess.GetMimeType(extention);
                response.StatusCode = statusCode;
                response.ContentLength64 = buffer.Length;
                var os = response.OutputStream;
                os.Write(buffer, 0, buffer.Length);
                os.Close();
                response.Close();
            }
            catch
            {
            }
        }

        private static void ResponseHtml(HttpListenerResponse response, string content, string title = "GFFHttpServer",
            int statusCode = 200)
        {
            var html = string.Format("<HTML><HEADER><TITLE>{0}</TITLE></HEADER><BODY><H3>O⌒O {1}</H3></BODY></HTML>",
                title, content);
            ResponseTxt(response, html, ".htm");
        }

        private static void ResponseFile(HttpListenerResponse response, string fileName, int statusCode = 200)
        {
            try
            {
                response.ContentType = MIMETypeProcess.GetMimeType(fileName);
                response.StatusCode = statusCode;
                var fs = File.ReadAllBytes(fileName);
                response.ContentLength64 = fs.LongLength;
                var os = response.OutputStream;
                os.Write(fs, 0, fs.Length);
                os.Close();
                fs = null;
                response.Close();
            }
            catch
            {
            }
        }
    }
}