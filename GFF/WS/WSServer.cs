/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：e9665ab3-0600-4f75-a7b5-aa5624d306b9
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS
 * 类名称：WSServer
 * 创建时间：2017/6/26 13:19:41
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Net;
using System.Text;
using System.Threading;
using GFF.Core.Handler;
using GFF.Core.Interface;
using GFF.Core.Tcp;
using GFF.Core.Tcp.Model;
using GFF.Core.Tcp.Server;
using GFF.Helper;
using GFF.MS.Model;
using GFF.WS.Model;


namespace GFF.WS
{
    /// <summary>
    /// websocket server
    /// </summary>
    public class WSServer : ServerSocket, IServer
    {
        string channelID = "wenli.chat";

        private static readonly ServerConfig ServerConfig = ServerConfig.Instance();

        private readonly Semaphore _receiveSemaphore;

        private readonly Semaphore _sendSemaphore;

        /// <summary>
        ///     websocket server
        /// </summary>
        public WSServer()
            : base(ServerConfig.MaxClientSize, ServerConfig.InitBufferSize)
        {
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
        /// ws与客户端建立连接事件
        /// </summary>
        public event OnWSConnectedHandler OnConnected;

        public override event OnServerErrorHandler OnErrored;

        protected override void RaiseOnErrored(Exception ex, params object[] args)
        {
            if (args != null && args[0] != null)
            {
                WSUserToken u = (WSUserToken)args[0];
            }
            OnErrored?.Invoke(ex, args);
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
            var wsUserToken = (WSUserToken)userToken;
            if (!wsUserToken.HandShaked)
            {
                string msg = Encoding.UTF8.GetString(args, 0, args.Length);
                if (msg.Contains("Sec-WebSocket-Key"))
                {
                    var data = WSFrame.PackageHandShakeData(args, args.Length);
                    wsUserToken.HandShaked = true;
                    base.SendAsync(wsUserToken, data);
                    wsUserToken.UID = Guid.NewGuid().ToString("N");
                    wsUserToken.ReceiveArgs.UserToken = wsUserToken;
                    UserTokenList.SetByChannelID(channelID, wsUserToken);
                    OnConnected?.BeginInvoke(_numConnectedSockets, wsUserToken, null, null);
                }
            }
            else
            {
                userToken.UnPackage(args, (bytes) =>
                {
                    OnSocketReceived?.BeginInvoke(userToken, bytes, null, null);

                    var list = UserTokenList.GetListByChannelID(channelID);

                    if (list != null)
                    {
                        foreach (var item in list)
                        {
                            base.SendAsync(item, WSFrame.PackageData(bytes));
                        }
                    }
                });


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
                base.Start(typeof(WSUserToken), ServerConfig.Instance().IP, ServerConfig.Instance().Port, ServerConfig.Instance().Backlog, ServerConfig.Instance().SocketTimeOutMS);
                IsStarted = true;
            }
        }

        public void Start(Uri uri)
        {
            if (!IsStarted)
            {
                //启动tcp服务
                var ip = string.Empty;
                foreach (var item in Dns.GetHostAddresses(uri.DnsSafeHost))
                {
                    ip = item.ToString();
                    if (ip != "::1")
                    {
                        break;
                    }
                }
                base.Start(typeof(WSUserToken), ip, uri.Port, ServerConfig.Instance().Backlog, ServerConfig.Instance().SocketTimeOutMS);
                IsStarted = true;
            }
        }
        public void Start(string url)
        {
            this.Start(new Uri(url));
        }

        public void SendAsync(IUserToken userToken, WSMessage message)
        {
            base.SendAsync(userToken, SerializeHelper.ProtolBufSerialize(message));
        }

        /// <summary>
        ///     停止服务器
        /// </summary>
        public void Stop()
        {
            if (IsStarted)
            {
                IsStarted = false;
            }
        }

    }
}
