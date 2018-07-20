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
using System.Net;
using System.Threading;
using GFF.Helper;

namespace GFF.Core.Http
{
    /// <summary>
    ///     构建一个Http服务器
    /// </summary>
    public class HttpServer : IDisposable
    {
        private bool _isStarted;

        private HttpListener listerner = new HttpListener();

        private MutiThreadHelper mutiThreadHelper = new MutiThreadHelper();

        /// <summary>
        ///     构建一个Http服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="auth"></param>
        public HttpServer(string ip = "127.0.0.1", int port = 8080,
            AuthenticationSchemes auth = AuthenticationSchemes.Anonymous)
        {
            listerner.AuthenticationSchemes = auth;
            listerner.Prefixes.Add("http://" + ip + ":" + port + "/");            
        }


        public void Dispose()
        {
            Stop();
            listerner = null;
            mutiThreadHelper = null;
        }

        /// <summary>
        ///     启动指定线程并发的服务
        /// </summary>
        /// <param name="threadNum"></param>
        public void Start(int threadNum = 10)
        {
            if (!_isStarted)
            {
                try
                {
                    listerner.Start();
                }
                catch
                {
                    throw new Exception("权限不足，请使用管理员运行！");
                }
                _isStarted = true;
                mutiThreadHelper.Start(threadNum, () =>
                {
                    while (_isStarted)
                    {
                        //等待请求连接
                        //没有请求则GetContext处于阻塞状态
                        var context = listerner.GetContext();
                        ThreadPool.QueueUserWorkItem(HttpTaskProcess.Process, context);
                    }                    
                });
            }
        }

        public void Stop()
        {
            _isStarted = false;
            mutiThreadHelper.Stop();
            listerner.Stop();
        }
    }
}