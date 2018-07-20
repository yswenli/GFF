/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：139ba3ad-a482-4c24-abcb-c79e0c00eb1a
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper
 * 类名称：TaskHelper
 * 创建时间：2017/1/10 14:36:57
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GFF.Helper
{
    /// <summary>
    /// 任务加强辅助类
    /// </summary>
    public static class TaskHelper
    {
        /// <summary>
        /// 启动异步任务
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static Task Run(Action action)
        {
            return Task.Factory.StartNew(action);
        }

        /// <summary>
        /// 任务超时时长为30秒
        /// </summary>
        /// <param name="action"></param>
        /// <param name="expreid"></param>
        /// <param name="failed"></param>
        public static void StartNew(Action action, int expreid = 30 * 1000, Action failed = null)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            var task = Task.Factory.StartNew(action, token);
            Task.Factory.StartNew(() =>
            {
                if (task != null && task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.Canceled)
                {
                    Thread.Sleep(expreid);
                    cancelTokenSource.Cancel();
                    failed?.Invoke();
                }
            });
        }
        /// <summary>
        /// 任务超时时长为30秒
        /// </summary>
        /// <param name="action"></param>
        /// <param name="expreid"></param>
        /// <param name="failed"></param>
        /// <param name="finall"></param>
        public static void StartNew(Action action, int expreid = 30 * 1000, Action failed = null, Action finall = null)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            var task = Task.Factory.StartNew(action, token);
            Task.Factory.StartNew(() =>
            {
                if (task != null && task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.Canceled)
                {
                    Thread.Sleep(expreid);
                    cancelTokenSource.Cancel();
                    failed?.Invoke();
                }
                finall?.Invoke();
            });
        }
        /// <summary>
        /// 任务超时时长为30秒
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="expreid"></param>
        /// <param name="failed"></param>
        public static void StartNew(Action<object> action, object state, int expreid = 30 * 1000, Action<object> failed = null)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            var task = Task.Factory.StartNew(action, state, token);
            Task.Factory.StartNew(() =>
            {
                if (task != null && task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.Canceled)
                {
                    Thread.Sleep(expreid);
                    cancelTokenSource.Cancel();
                    failed?.Invoke(state);
                }
            });
        }
        /// <summary>
        /// 任务超时时长为30秒
        /// </summary>
        /// <param name="action"></param>
        /// <param name="state"></param>
        /// <param name="expreid"></param>
        /// <param name="failed"></param>
        /// <param name="finall"></param>
        public static void StartNew(Action<object> action, object state, int expreid = 30 * 1000, Action<object> failed = null, Action finall = null)
        {
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;
            var task = Task.Factory.StartNew(action, state, token);
            Task.Factory.StartNew(() =>
            {
                if (task != null && task.Status != TaskStatus.RanToCompletion && task.Status != TaskStatus.Faulted && task.Status != TaskStatus.Canceled)
                {
                    Thread.Sleep(expreid);
                    cancelTokenSource.Cancel();
                    failed?.Invoke(state);
                }
                finall?.Invoke();
            });
        }
    }
}
