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
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace GFF.Helper
{
    /// <summary>
    ///     任务处理帮助类
    /// </summary>
    public static class FunHelper
    {
        static FunHelper()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Action a;
                    ActionQueue.TryDequeue(out a);
                    a?.Invoke();
                    if (a == null)
                        Thread.Sleep(1);
                }
            });
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Func<string> f;
                    FunQueue.TryDequeue(out f);
                    f?.Invoke();
                    if (f == null)
                        Thread.Sleep(1);
                }
            });
        }

        public static void OneceAsync(Action funcation)
        {
            Task.Factory.StartNew(() => { funcation(); });
        }

        private static void SingleWhileAsync(Action funcation, int mil)
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    funcation();
                    Thread.Sleep(mil);
                }
            });
        }

        public static void WhileAsync(Action funcation, int mil, bool muti = false)
        {
            if (!muti)
                SingleWhileAsync(funcation, mil);
            else
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        Task.Factory.StartNew(() => { funcation(); });
                        Thread.Sleep(mil);
                    }
                });
        }

        public static void ForAsync(Action funcation, int num)
        {
            Task.Factory.StartNew(() => { Parallel.For(0, num, i => { funcation(); }); });
        }

        #region Queues

        /// <summary>
        ///     无返回值的任务队列
        /// </summary>
        public static ConcurrentQueue<Action> ActionQueue { get; } = new ConcurrentQueue<Action>();

        /// <summary>
        ///     含返回值的任务队列
        /// </summary>
        public static ConcurrentQueue<Func<string>> FunQueue { get; } = new ConcurrentQueue<Func<string>>();

        #endregion
    }
}