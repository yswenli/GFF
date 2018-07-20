/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：66d4606e-25d4-4df6-acd5-c0c054710ec8
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：PingHelper
 * 创建时间：2016/11/11 16:51:28
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;

namespace GFF.Helper
{
    /// <summary>
    /// Ping工具类
    /// </summary>
    public class PingHelper
    {
        static IPStatus ipStatus;
        /// <summary>
        /// 返回的Ping信息
        /// </summary>
        public static IPStatus IPStatus
        {
            get
            {
                return ipStatus;
            }
        }

        static long roundtripTime;
        /// <summary>
        /// 返回超时情况
        /// </summary>
        public static long RoundtripTime
        {
            get
            {
                return roundtripTime;
            }
        }

        static int errorCount;
        /// <summary>
        /// 网络异常
        /// </summary>
        public static int ErrorCount
        {
            get
            {
                return errorCount;
            }
        }


        static Thread thread;

        static bool _Started = false;


        /// <summary>
        /// 启动Ping
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="timeout"></param>
        public static void Start(string ip, int timeout = 1000)
        {
            string data = "Ping Data";
            byte[] buffer = Encoding.UTF8.GetBytes(data);
            PingHelper.Start(ip, timeout, buffer);
        }

        /// <summary>
        ///  启动Ping
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="timeout"></param>
        /// <param name="buffer"></param>
        /// <param name="callBack"></param>
        public static void Start(string ip, int timeout, byte[] buffer, Action callBack = null)
        {
            if (!_Started)
            {
                _Started = true;
                thread = new Thread(() =>
                {
                    while (true)
                    {
                        try
                        {
                            Ping p = new Ping();
                            PingOptions options = new PingOptions();
                            options.DontFragment = true;
                            PingReply reply = p.Send(ip, timeout, buffer, options);
                            ipStatus = reply.Status;
                            if (reply.Status == IPStatus.Success)
                            {
                                roundtripTime = reply.RoundtripTime;
                            }
                            else
                            {
                                roundtripTime = -1;
                                errorCount++;
                                errorAlertFlag = true;
                            }
                        }
                        catch
                        {
                            ipStatus = IPStatus.BadRoute;
                            roundtripTime = -1;
                            errorCount++;
                            errorAlertFlag = true;
                        }
                        finally
                        {
                            Thread.Sleep(timeout + 500);
                        }
                        if (callBack != null)
                            callBack();
                    }
                });
                thread.Start();
            }
            
        }

        /// <summary>
        /// 停止Ping
        /// </summary>
        public static void Stop()
        {
            _Started = false;
        }

        static bool errorAlertFlag;
        /// <summary>
        /// 触发网络异常
        /// </summary>
        /// <param name="action"></param>
        public static void ErrorAlert(Action callBack)
        {
            new Thread(() =>
            {
                while (true)
                {
                    if (errorAlertFlag)
                    {
                        errorAlertFlag = false;
                        if (callBack != null)
                        {
                            callBack();
                        }
                    }
                    Thread.Sleep(10000);
                }
            }).Start();

        }

        //

    }
}
