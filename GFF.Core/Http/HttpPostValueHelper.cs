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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace GFF.Core.Http
{
    /// <summary>
    ///     获取Post请求中的参数和值帮助类
    /// </summary>
    public class HttpPostValueHelper : IDisposable
    {
        private HttpListenerRequest _request;

        public HttpPostValueHelper(HttpListenerRequest request)
        {
            _request = request;
        }


        public void Dispose()
        {
            if ((_request != null) && (_request.InputStream != null))
            {
                _request.InputStream.Dispose();
                _request = null;
            }
        }

        private bool CompareBytes(byte[] source, byte[] comparison)
        {
            try
            {
                var count = source.Length;
                if (source.Length != comparison.Length)
                    return false;
                for (var i = 0; i < count; i++)
                    if (source[i] != comparison[i])
                        return false;
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     读取内容
        ///     每次一行
        /// </summary>
        /// <param name="SourceStream"></param>
        /// <param name="position"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private byte[] ReadLineAsBytes(Stream SourceStream, int position, out int length)
        {
            length = 0;
            SourceStream.Position = position;
            using (var resultStream = new MemoryStream())
            {
                while (true)
                {
                    var data = SourceStream.ReadByte();
                    resultStream.WriteByte((byte) data);
                    length++;
                    if (data == 10)
                        break;
                }
                return resultStream.ToArray();
            }
        }

        /// <summary>
        ///     获取Post过来的参数和数据
        /// </summary>
        /// <returns></returns>
        public List<HttpPostItem> GetHttpPostItemList()
        {
            try
            {
                var HttpListenerPostValueList = new List<HttpPostItem>();
                if ((_request.ContentType.Length > 20) &&
                    (string.Compare(_request.ContentType.Substring(0, 20), "multipart/form-data;", true) == 0))
                    using (var sourceStream = new MemoryStream())
                    {
                        _request.InputStream.CopyTo(sourceStream);
                        sourceStream.Position = 0;

                        var HttpListenerPostValue = _request.ContentType.Split(';').Skip(1).ToArray();
                        var boundary = string.Join(";", HttpListenerPostValue).Replace("boundary=", "").Trim();
                        var ChunkBoundary = _request.ContentEncoding.GetBytes("--" + boundary + "\r\n");
                        var EndBoundary = _request.ContentEncoding.GetBytes("--" + boundary + "--\r\n");

                        var resultStream = new MemoryStream();
                        var CanMoveNext = true;
                        HttpPostItem data = null;

                        var position = 0;
                        var length = 0;
                        while (CanMoveNext)
                        {
                            var currentChunk = ReadLineAsBytes(sourceStream, position, out length);
                            position += length;
                            if (!_request.ContentEncoding.GetString(currentChunk).Equals("\r\n"))
                                resultStream.Write(currentChunk, 0, currentChunk.Length);
                            if (CompareBytes(ChunkBoundary, currentChunk))
                            {
                                var result = new byte[resultStream.Length - ChunkBoundary.Length];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                CanMoveNext = true;
                                if (result.Length > 0)
                                    data.datas = result;
                                data = new HttpPostItem();
                                HttpListenerPostValueList.Add(data);
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (_request.ContentEncoding.GetString(currentChunk).Contains("Content-Disposition"))
                            {
                                var result = new byte[resultStream.Length - 2];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                CanMoveNext = true;
                                data.name =
                                    new Regex("filename=\"(?<fn>.*)\"").Match(Encoding.UTF8.GetString(result)).Groups[
                                        "fn"].Value;
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (_request.ContentEncoding.GetString(currentChunk).Contains("Content-Type"))
                            {
                                CanMoveNext = true;
                                data.type = 1;
                                resultStream.Dispose();
                                resultStream = new MemoryStream();
                            }
                            else if (CompareBytes(EndBoundary, currentChunk))
                            {
                                var result = new byte[resultStream.Length - EndBoundary.Length - 2];
                                resultStream.Position = 0;
                                resultStream.Read(result, 0, result.Length);
                                data.datas = result;
                                resultStream.Dispose();
                                CanMoveNext = false;
                            }
                            currentChunk = null;
                        }
                        if (resultStream != null)
                            resultStream.Dispose();
                        _request.InputStream.Close();
                    }
                return HttpListenerPostValueList;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}