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
using System.Threading;

namespace GFF.Helper
{
    /// <summary>
    ///     多线程处理类
    /// </summary>
    public class MutiThreadHelper : IDisposable
    {
        private List<Thread> list;

        public MutiThreadHelper()
        {
            list = new List<Thread>();
        }

        public int Count { get; private set; }

        public bool IsStarted { get; private set; }

        public void Dispose()
        {
            Stop();
        }

        /// <summary>
        ///     多线程任务启动
        /// </summary>
        /// <param name="num"></param>
        /// <param name="action"></param>
        public void Start(int num, Action action)
        {
            if (num > 0)
                if (!IsStarted)
                {
                    Count = num;
                    IsStarted = true;
                    for (var i = 0; i < num; i++)
                    {
                        var th = new Thread(new ThreadStart(action));
                        th.IsBackground = true;
                        th.Start();
                        list.Add(th);
                    }
                }
        }

        /// <summary>
        ///     多线程任务停止
        /// </summary>
        public void Stop()
        {
            if (IsStarted)
            {
                foreach (var td in list)
                    if (td != null)
                        td.Abort();
                IsStarted = false;
                Count = 0;
                list.Clear();
            }
        }
    }
}