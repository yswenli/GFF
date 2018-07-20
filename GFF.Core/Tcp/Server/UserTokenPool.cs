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
using System.Net.Sockets;
using GFF.Core.Interface;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;

namespace GFF.Core.Tcp.Server
{
    /// <summary>
    ///     用户列表复用池
    /// </summary>
    public class UserTokenPool
    {
        private static ConcurrentQueue<IUserToken> pool = new ConcurrentQueue<IUserToken>();

        /// <summary>
        ///     用户列表复用池
        /// </summary>
        /// <param name="max"></param>
        public UserTokenPool(Type type, int max, int initBufferSize, EventHandler<SocketAsyncEventArgs> IOEvent)
        {
            for (var i = 0; i < max; i++)
            {
                var userToken = (IUserToken)Activator.CreateInstance(type);
                userToken.ReceiveArgs.Completed += IOEvent;
                userToken.ReceiveArgs.UserToken = userToken;
                pool.Enqueue(userToken);
            }
        }

        public void Push(IUserToken userToken)
        {
            pool.Enqueue(userToken);
        }

        public IUserToken Pop()
        {
            IUserToken userToken = null;
            pool.TryDequeue(out userToken);
            return userToken;
        }

        public int Count()
        {
            return pool.Count;
        }
    }
}