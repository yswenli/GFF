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

using System.Collections.Generic;

namespace GFF.Core.Http
{
    /// <summary>
    ///     MIMEType处理类
    /// </summary>
    public static class MIMETypeProcess
    {
        private static readonly Dictionary<string, string> collection = new Dictionary<string, string>();

        static MIMETypeProcess()
        {
            collection.Add(".*", "application/octet-stream");
            collection.Add(".tif", "image/tiff");
            collection.Add(".aif", "audio/aiff");
            collection.Add(".aifc", "audio/aiff");
            collection.Add(".aiff", "audio/aiff");
            collection.Add(".asf", "video/x-ms-asf");
            collection.Add(".asp", "text/asp");
            collection.Add(".asx", "video/x-ms-asf");
            collection.Add(".au", "audio/basic");
            collection.Add(".avi", "video/avi");
            collection.Add(".bmp", "application/x-bmp");
            collection.Add(".txt", "text/html;charset=UTF-8");
            collection.Add(".html", "text/html;charset=UTF-8");
            collection.Add(".htm", "text/html;charset=UTF-8");
            collection.Add(".htx", "text/html;charset=UTF-8");
            collection.Add(".mhtml", "text/html;charset=UTF-8");
            collection.Add(".ico", "image/x-icon");
            collection.Add(".jpe", "image/jpeg");
            collection.Add(".jpeg", "image/jpeg");
            collection.Add(".jpg", "image/jpeg");
            collection.Add(".gif", "image/gif");
            collection.Add(".js", "application/x-javascript");
            collection.Add(".jsp", "text/html");
            collection.Add(".mp4", "video/mpeg4");
            collection.Add(".mpeg", "video/mpg");
            collection.Add(".mpg", "video/mpg");
            collection.Add(".mpv", "video/mpg");
            collection.Add(".mpv2", "video/mpg");
            collection.Add(".png", "application/x-png");
            collection.Add(".wmv", "video/x-ms-wmv");
            collection.Add(".xhtml", "text/html");
            collection.Add(".xml", "text/html");
            collection.Add(".apk", "application/vnd.android.package-archive");
            collection.Add(".css", "text/css");
        }

        public static string GetMimeType(string fileName)
        {
            var result = "application/octet-stream";
            try
            {
                if (!string.IsNullOrEmpty(fileName))
                    if (fileName.IndexOf(".") > -1)
                    {
                        var extentionName = fileName.Substring(fileName.LastIndexOf("."));
                        if (collection.ContainsKey(extentionName))
                            result = collection[extentionName];
                    }
            }
            catch
            {
            }
            return result;
        }
    }
}