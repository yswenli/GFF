/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：GFF.Core.Tool
 * 类名称：ServerSocket
 * 文件名：ServerSocket
 * 创建年份：2016
 * 创建时间：2016/11/1 13:25:46
 * 创建人：Administrator
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using GFF.Core.Http;
using GFF.Core.Tcp.Model;
using GFF.Core.Tcp.Server;
using GFF.Helper;
using GFF.Model.Entity;
using GFF.Core.Handler;
using GFF.Core.Interface;

namespace GFF.Core.Tcp
{
    /// <summary>
    ///    Server Socket操作类
    /// </summary>    
    public abstract class ServerSocket
    {
        private Socket _listenSock; // 用于侦听传入的连接请求的套接字   

        protected int _numConnectedSockets;

        private BufferManager _bufferManager;

        private int _maxClient = 1000;

        private int _bufferSize = 1024;

        public UserTokenPool MyUserTokenPool
        {
            get; set;
        }

        private HttpServer httpServer;

        #region Event

        public virtual event OnAcceptedHandler OnAccepted;

        protected virtual void RaiseOnAccepted(int num, IUserToken userToken)
        {
            OnAccepted?.BeginInvoke(num, userToken, null, null);
        }

        /// <summary>
        ///     客户端从服务器断开
        /// </summary>
        public virtual event OnUnAcceptedHandler OnUnAccepted;

        /// <summary>
        /// 服务器socket接收到数据
        /// </summary>
        public virtual event OnSocketReceivedMessageHandler OnSocketReceived;
        /// <summary>
        /// 服务器socket接收到数据
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="args"></param>
        protected virtual void RaiseOnSocketReceived(IUserToken userToken, byte[] args)
        {
            OnSocketReceived?.BeginInvoke(userToken, args, null, null);
        }

        /// <summary>
        ///     服务处理异常
        /// </summary>
        public virtual event OnServerErrorHandler OnErrored;

        protected virtual void RaiseOnErrored(Exception ex, params object[] args)
        {
            OnErrored?.BeginInvoke(ex, args, null, null);
        }
        #endregion

        public ServerSocket(int numConnections, int initBufferSize)
        {
            _maxClient = numConnections;

            _bufferSize = initBufferSize;

            _numConnectedSockets = 0;

            _bufferManager = new BufferManager(_bufferSize * _maxClient, _bufferSize);

        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="localEndPoint"></param>
        /// <param name="userTokenType"></param>
        /// <param name="backlog"></param>
        /// <param name="timeOut"></param>
        protected void Start(IPEndPoint localEndPoint, Type userTokenType, int backlog = 100000, int timeOut = 2 * 60)
        {
            _bufferManager.InitBuffer();
            // 预分配的UserToken对象池
            MyUserTokenPool = new UserTokenPool(userTokenType, _maxClient, _bufferSize, Operation_Completed);
            //socket初始化
            _listenSock = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);            
            _listenSock.SendTimeout = _listenSock.ReceiveTimeout = timeOut;
            _listenSock.ReceiveBufferSize = _bufferSize;
            _listenSock.Bind(localEndPoint);
            _listenSock.Listen(backlog);
            StartAccept(null);
        }

        /// <summary>
        /// 启动服务
        /// </summary>
        /// <param name="userTokenType"></param>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <param name="backlog"></param>
        /// <param name="timeOut"></param>
        protected void Start(Type userTokenType, string ip = "127.0.0.1", int port = 6666, int backlog = 100000, int timeOut = 2 * 60)
        {
            Start(new IPEndPoint(IPAddress.Parse(ip), port), userTokenType, backlog, timeOut);
        }

        /// <summary>
        ///     开始一个操作以接受来自客户端的连接请求
        ///     SocketAsyncEventArgs这个是一个重用的对象
        ///     和其他的SocketAsyncEventArgs没有直接的关系
        /// </summary>
        /// <param name="acceptEventArg"></param>
        private void StartAccept(SocketAsyncEventArgs acceptEventArg)
        {
            if (acceptEventArg == null)
            {
                acceptEventArg = new SocketAsyncEventArgs();
                acceptEventArg.Completed += AcceptEventArg_Completed;
            }
            else
            {
                // 上下文对象正在被重用时必须清除套接字
                acceptEventArg.AcceptSocket = null;
            }
            var willRaiseEvent = _listenSock.AcceptAsync(acceptEventArg);
            if (!willRaiseEvent)
                ProcessAccept(acceptEventArg);
        }

        /// <summary>
        ///     异步接受连接方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEventArg_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        /// <summary>
        ///     处理接受客户端的连接
        /// </summary>
        /// <param name="acceptEventArgs"></param>
        private void ProcessAccept(SocketAsyncEventArgs acceptEventArgs)
        {
            try
            {
                var remoteSocket = acceptEventArgs.AcceptSocket;//和客户端关联的socket

                if (remoteSocket.Connected)
                {
                    TaskHelper.Run(() =>
                    {
                        IUserToken userToken = null;
                        try
                        {
                            Interlocked.Increment(ref _numConnectedSockets);//原子操作加1
                            userToken = MyUserTokenPool.Pop(); //将对象池中的一个空闲对象取出与当前的用户socket绑定
                            userToken.ConnectDateTime = DateTime.Now;
                            userToken.ConnectSocket = remoteSocket;
                            _bufferManager.SetBuffer(userToken.ReceiveArgs);// 指定从缓冲池字节缓冲区的socketasynceventarg对象
                            userToken.ReceiveArgs.UserToken = userToken;
                            if (!remoteSocket.ReceiveAsync(userToken.ReceiveArgs))//投递接收请求
                            {
                                ProcessReceive(userToken.ReceiveArgs);
                            }
                            RaiseOnAccepted(_numConnectedSockets, userToken);
                        }
                        catch (SocketException ex)
                        {
                            RaiseOnErrored(new Exception(String.Format("接受客户 {0} 连接出错, 异常信息： {1} .", userToken, ex.ToString())));
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                RaiseOnErrored(ex);
            }
            StartAccept(acceptEventArgs);
        }

        private void Operation_Completed(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        ProcessReceive(e);
                        break;
                    case SocketAsyncOperation.Send:
                        ProcessSend(e);
                        break;
                    default:
                        throw new ArgumentException("在套接字上完成的最后一个操作不是接收或发送,IO_Completed.SocketAsyncOperation:" +
                                                    e.LastOperation);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e, "在套接字上完成的最后一个操作不是接收或发送,IO_Completed.SocketAsyncOperation:" + e.LastOperation, ex);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            try
            {
                // 检查远程主机是否关闭连接
                if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
                {
                    var userToken = (IUserToken)e.UserToken;

                    userToken.ActiveDateTime = DateTime.Now;

                    var buffer = new byte[e.BytesTransferred];

                    Buffer.BlockCopy(userToken.ReceiveArgs.Buffer, e.Offset, buffer, 0, buffer.Length);

                    RaiseOnSocketReceived(userToken, buffer);

                    Array.Clear(buffer, 0, buffer.Length);

                    buffer = null;

                    if (userToken.ConnectSocket.Connected)
                    {
                        var willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveArgs);
                        if (!willRaiseEvent)
                            ProcessReceive(userToken.ReceiveArgs);
                    }
                    else
                    {
                        CloseClientSocket(e, "网络连接已断开");
                    }
                }
                else
                {
                    CloseClientSocket(e, "消息处理发生异常，已断开连接", new Exception("消息处理发生异常，已断开连接,e.SocketError:" + e.SocketError));
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(e, "消息处理发生异常，已断开连接", ex);
            }
        }

        private bool ProcessSend(SocketAsyncEventArgs e)
        {
            var userToken = (IUserToken)e.UserToken;
            userToken.ActiveDateTime = DateTime.Now;
            try
            {
                if (e.SocketError == SocketError.Success)
                {
                    return true;
                }
                CloseClientSocket(e, "发送数据socket异常：" + e.SocketError);
            }
            finally
            {
                e.AcceptSocket = null;
                e.Dispose();
            }
            return false;
        }

        

        /// <summary>
        /// 通用发送
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="msg"></param>
        protected void SendAsync(IUserToken userToken, byte[] msg)
        {
            if (!userToken.ConnectSocket.Connected)
            {
                CloseClientSocket(userToken.ReceiveArgs, "网络已断开，发送数据socket异常：" + userToken.ReceiveArgs.SocketError);
                return;
            }
            var sendArgs = new SocketAsyncEventArgs();
            try
            {
                if (sendArgs != null)
                {
                    sendArgs.AcceptSocket = userToken.ConnectSocket;
                    sendArgs.Completed += Operation_Completed;
                    sendArgs.UserToken = userToken;
                    sendArgs.SetBuffer(msg, 0, msg.Length);
                    var willRaiseEvent = userToken.ConnectSocket.SendAsync(sendArgs);
                    if (!willRaiseEvent)
                        ProcessSend(sendArgs);
                }
            }
            catch (Exception ex)
            {
                CloseClientSocket(sendArgs, "", ex);
            }
        }

        /// <summary>
        ///     服务器主动断开客户端连接
        /// </summary>
        /// <param name="e"></param>
        public void CloseClientSocket(SocketAsyncEventArgs e, string msg, Exception ex = null)
        {
            var userToken = e.UserToken as IUserToken;

            if (userToken != null)
            {
                if (userToken.ConnectSocket != null)
                {
                    if (userToken.ConnectSocket.Connected)
                    {
                        userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
                        userToken.ConnectSocket.Close();
                    }
                }
                // 减少连接到服务器的客户端的总数的计数器跟踪
                if (_numConnectedSockets > 0)
                {
                    Interlocked.Decrement(ref _numConnectedSockets);

                    TaskHelper.Run(() =>
                    {
                        OnUnAccepted?.BeginInvoke(_numConnectedSockets, userToken, null, null);
                    });
                }

                MyUserTokenPool.Push(userToken);

                UserTokenList.DelByUID(userToken);

                if (ex != null)
                    RaiseOnErrored(ex, userToken, msg);
            }
        }


    }
}