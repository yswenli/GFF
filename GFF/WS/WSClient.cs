/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：d1eb1184-57f2-42b6-a262-8ac73c0698a8
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS
 * 类名称：WSClient
 * 创建时间：2017/6/26 17:09:31
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using GFF.Core.Tcp;
using GFF.Core.Tcp.Model;
using GFF.WS.Model;



namespace GFF.WS
{
    public class WSClient : IDisposable
    {
        ClientSocket _client;

        private string _serverIP;

        private int _serverPort;

        public bool HandShaked
        {
            get; private set;
        }

        public WSClient()
        {
            _client = new ClientSocket(typeof(WSUserToken), ClientConfig.Instance().InitBufferSize, ClientConfig.Instance().SocketTimeOutMS, string.Empty);
            _client.OnConnected += _client_OnConnected;
            _client.OnData += _client_OnData;
        }

        private void _client_OnData(object sender, byte[] data)
        {
            var userToken = (WSUserToken)_client.ReceiveSocket.UserToken;

            if (!userToken.HandShaked)
            {
                userToken.HandShaked = true;
                OnConnected?.Invoke(this);
            }
            else
            {
                userToken.UnPackage(data, (rdata)=>
                {
                    OnReceived?.Invoke(rdata);

                    OnReceivedMessage?.Invoke(Helper.SerializeHelper.ProtolBufDeserialize<Model.WSMessage>(rdata));
                });
            }
        }

        private void _client_OnConnected(object sender)
        {
            _client.SendAsync(WSFrame.PackageClientHandShark(_serverIP, _serverPort));
        }

        #region event
        public event Action<WSClient> OnConnected;
        public event Action<byte[]> OnReceived;
        public event Action<Model.WSMessage> OnReceivedMessage;
        #endregion

        #region 公开方法
        public void DisConnect()
        {
            _client.DisConnect();
        }


        /// <summary>
        ///     关闭连接
        /// </summary>
        public void Dispose()
        {
            if (_client != null)
                _client.DisConnect();
        }

        /// <summary>
        ///     连接到服务器
        /// </summary>
        public void ConnectAsync()
        {
            _client.Connect(ClientConfig.Instance().IP, ClientConfig.Instance().Port);
        }

        public void ConnectAsync(Uri uri)
        {
            var ip = string.Empty;
            foreach (var item in Dns.GetHostAddresses(uri.DnsSafeHost))
            {
                ip = item.ToString();
                if (ip != "::1")
                {
                    break;
                }
            }
            _serverIP = ip;
            _serverPort = uri.Port;
            _client.Connect(_serverIP, _serverPort);
        }
        public void ConnectAsync(string wsurl)
        {
            this.ConnectAsync(new Uri(wsurl));
        }

        public void SendAsync(byte[] msg)
        {
            _client.SendAsync(WSFrame.PackageData(msg));
        }

        public void SendAsync(string msg)
        {
            this.SendAsync(Encoding.UTF8.GetBytes(msg));
        }
        public void SendAsync(Model.WSMessage msg)
        {
            this.SendAsync(Helper.SerializeHelper.ProtolBufSerialize(msg));
        }
        #endregion
    }
}
