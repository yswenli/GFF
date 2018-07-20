/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：cb23cf8c-ec57-4701-951c-d5e8e9ab06b0
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF
 * 类名称：MessageServer
 * 创建时间：2016/11/4 17:42:09
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GFF.Core.Handler;
using GFF.Core.Http;
using GFF.Core.Interface;
using GFF.Core.Tcp;
using GFF.Core.Tcp.Model;
using GFF.Core.Tcp.Server;
using GFF.Helper;
using GFF.Model.Entity;
using GFF.Model.Enum;
using GFF.MS.Model;

namespace GFF.MS
{
    public class MessageServer : ServerSocket, IServer
    {
        private static readonly ServerConfig ServerConfig = ServerConfig.Instance();

        private readonly HttpServer _httpServer;
        private readonly Semaphore _receiveSemaphore;
        private readonly Semaphore _sendSemaphore;

        /// <summary>
        ///     服务器类
        /// </summary>
        public MessageServer()
            : base(ServerConfig.MaxClientSize, ServerConfig.InitBufferSize)
        {
            //启动http服务
            _httpServer = new HttpServer(ServerConfig.IP, ServerConfig.HttpPort);
            //加入限制
            _receiveSemaphore = new Semaphore(10000 * ServerConfig.Instance().OperationThreads, 100000 * ServerConfig.Instance().OperationThreads);
            _sendSemaphore = new Semaphore(10000 * ServerConfig.Instance().OperationThreads, 100000 * ServerConfig.Instance().OperationThreads);
        }

        /// <summary>
        ///     是否启动
        /// </summary>
        public bool IsStarted
        {
            get; private set;
        }

        /// <summary>
        ///     在线客户端数
        /// </summary>
        public int OnlineNums
        {
            get
            {
                return _numConnectedSockets;
            }
        }
        /// <summary>
        /// 服务器socket接收到数据
        /// </summary>
        public override event OnSocketReceivedMessageHandler OnSocketReceived;
        /// <summary>
        /// 服务器socket接收到数据
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="args"></param>
        protected override void RaiseOnSocketReceived(IUserToken userToken, byte[] args)
        {
            OnSocketReceived?.BeginInvoke(userToken, args, null, null);

            userToken.UnPackage(args, (p) =>
            {
                try
                {
                    RaiseOnReceived(userToken, SerializeHelper.ProtolBufDeserialize<Message>(p));
                }
                catch (Exception ex)
                {
                    CloseClientSocket(userToken.ReceiveArgs, "消息处理发生异常，已断开连接", ex);
                }
            });
        }
        /// <summary>
        ///     收取消息事件
        /// </summary>
        public event OnServerReceivedMessageHandler OnServerReceived;

        /// <summary>
        ///     收取消息后入队
        /// </summary>
        /// <param name="userToken"></param>
        /// <param name="msg"></param>
        protected void RaiseOnReceived(IUserToken userToken, Message msg)
        {
            msg.SendTick = DateTime.Now.Ticks;

            ProcessReceivedMsg(userToken, msg);

            OnServerReceived?.Invoke(userToken, msg);
        }

        public override event OnServerErrorHandler OnErrored;

        protected override void RaiseOnErrored(Exception ex, params object[] args)
        {
            if (args != null && args[0] != null)
            {
                UserToken u = (UserToken)args[0];
            }
            OnErrored?.Invoke(ex, args);
        }

        /// <summary>
        ///     发送消息到客户端
        /// </summary>
        /// <param name="sendArgs"></param>
        /// <param name="msg"></param>
        protected void SendMsgAsync(IUserToken userToken, Message msg)
        {
            using (var p = new TcpPackage(SerializeHelper.ProtolBufSerialize(msg)))
            {
                this.SendAsync(userToken, p.Data);
            }
        }

        /// <summary>
        ///     发送频道消息
        /// </summary>
        /// <param name="msg"></param>
        protected void SendChannelMsg(Message msg)
        {
            if (!UserTokenList.IsEmpty && (msg != null) && (msg.Data != null))
            {
                var list = UserTokenList.GetListByChannelID(msg.Accepter);
                if (list.Count > 0)
                    Parallel.ForEach(list, userToken =>
                    {
                        ProcessSendMsg(userToken, msg);
                    });
            }
        }
        protected void SendHeart(Message msg)
        {
            if (!UserTokenList.IsEmpty && (msg != null))
            {
                var userToken = UserTokenList.GetUserTokenByUID(msg.Sender);
                if (userToken != null)
                    ProcessSendMsg(userToken, msg);
            }
        }
        /// <summary>
        ///     发送私信
        /// </summary>
        /// <param name="msg"></param>
        protected void SendPrivateMsg(Message msg)
        {
            if (!UserTokenList.IsEmpty && (msg != null))
            {
                var userToken = UserTokenList.GetUserTokenByUID(msg.Accepter);
                if (userToken != null)
                    ProcessSendMsg(userToken, msg);
            }
        }

        protected void SendFile(Message msg)
        {
            if (!UserTokenList.IsEmpty && (msg != null))
            {
                var userToken = UserTokenList.GetUserTokenByUID(msg.Accepter);
                if (userToken != null)
                    ProcessSendMsg(userToken, msg);
            }
        }

        /// <summary>
        ///     启动服务器
        /// </summary>
        public void Start()
        {
            if (!IsStarted)
            {
                //启动tcp服务
                base.Start(typeof(UserToken), ServerConfig.Instance().IP, ServerConfig.Instance().Port, ServerConfig.Instance().Backlog, ServerConfig.Instance().SocketTimeOutMS);

                IsStarted = true;

                //启动http服务
                _httpServer.Start(ServerConfig.Instance().OperationThreads);
            }
        }

        /// <summary>
        ///     处理转发队列
        /// </summary>
        private void ProcessReceivedMsg(IUserToken userToken, Message msg)
        {
            _receiveSemaphore.WaitOne();
            try
            {
                if (msg != null)
                    switch (msg.Protocal)
                    {
                        case (byte)MessageProtocalEnum.Heart:
                            msg.Accepter = msg.Sender;
                            SendHeart(msg);
                            break;
                        case (byte)MessageProtocalEnum.Login:
                            userToken.UID = msg.Accepter;
                            UserTokenList.SetByUID(userToken);
                            msg.Protocal = (byte)MessageProtocalEnum.RLogin;
                            msg.Data = Encoding.UTF8.GetBytes("登录成功");
                            SendPrivateMsg(msg);
                            break;
                        case (byte)MessageProtocalEnum.Logout:
                            userToken.UID = msg.Accepter;
                            UserTokenList.DelByUID(userToken);
                            break;
                        case (byte)MessageProtocalEnum.Subscribe:
                            userToken.UID = msg.Sender;
                            UserTokenList.SetByChannelID(msg.Accepter, userToken);
                            msg.Protocal = (byte)MessageProtocalEnum.RSubscribe;
                            msg.Data = Encoding.UTF8.GetBytes("订阅成功");
                            SendPrivateMsg(msg);
                            break;
                        case (byte)MessageProtocalEnum.Unsubscribe:
                            userToken.UID = msg.Accepter;
                            UserTokenList.DelByChannelID(msg.Accepter, userToken);
                            break;
                        case (byte)MessageProtocalEnum.Message:
                            SendChannelMsg(msg);
                            break;
                        case (byte)MessageProtocalEnum.PrivateMsg:
                            SendPrivateMsg(msg);
                            break;
                        case (byte)MessageProtocalEnum.File:
                            SendFile(msg);
                            break;
                    }
            }
            catch (Exception ex)
            {
                RaiseOnErrored(ex, userToken, msg);
            }
            _receiveSemaphore.Release();

        }

        /// <summary>
        ///     处理发送队列
        /// </summary>
        private void ProcessSendMsg(IUserToken userToken, Message msg)
        {
            _sendSemaphore.WaitOne();
            try
            {
                try
                {
                    if ((userToken != null) && (msg != null))
                        SendMsgAsync(userToken, msg);
                }
                catch (Exception ex)
                {
                    RaiseOnErrored(ex, userToken, msg);
                }

            }
            catch (Exception ex)
            {
                RaiseOnErrored(ex, userToken, msg);
            }
            _sendSemaphore.Release();
        }

        /// <summary>
        ///     停止服务器
        /// </summary>
        public void Stop()
        {
            if (IsStarted)
            {
                _httpServer.Stop();
                IsStarted = false;
            }
        }
    }
}