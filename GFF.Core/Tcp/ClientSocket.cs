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
using System.Net.Sockets;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;
using GFF.Core.Handler;
using GFF.Core.Interface;

namespace GFF.Core.Tcp
{
    /// <summary>
    ///     客户端socket操作
    /// </summary>
    public class ClientSocket
    {
        private readonly int _bufferSize;

        private readonly int _timeOut;

        private Socket clientSocket;

        private object lockObj = new object();

        public SocketAsyncEventArgs ReceiveSocket;

        Type _userTokenType;


        public ClientSocket(Type userTokenType, int bufferSize, int timeOut, string uid = "")
        {
            _userTokenType = userTokenType;
            _bufferSize = bufferSize;
            _timeOut = timeOut;
            if (string.IsNullOrEmpty(uid))
                UID = Guid.NewGuid().ToString("N");
            else
                UID = uid;
        }

        public string UID
        {
            get; set;
        }

        public bool IsConnected
        {
            get; set;
        }

        public void Connect(string ip = "127.0.0.1", int port = 6666)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Blocking = false;
            clientSocket.SendTimeout = clientSocket.ReceiveTimeout = _timeOut;

            var connectsocketAsync = new SocketAsyncEventArgs();
            connectsocketAsync.Completed += ConnectsocketAsync_Completed;
            connectsocketAsync.RemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            if (!clientSocket.ConnectAsync(connectsocketAsync))
                Connected(connectsocketAsync);
        }


        private void ConnectsocketAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            Connected(e);
        }

        private void Connected(SocketAsyncEventArgs connectsocketAsync)
        {
            if (connectsocketAsync.SocketError == SocketError.Success)
            {
                ReceiveSocket = new SocketAsyncEventArgs();
                var buffer = new byte[_bufferSize];
                ReceiveSocket.SetBuffer(buffer, 0, _bufferSize);
                ReceiveSocket.Completed += SocketIO_Completed;
                ReceiveSocket.AcceptSocket = connectsocketAsync.ConnectSocket;

                var userToken = (IUserToken)Activator.CreateInstance(_userTokenType);
                userToken.ReceiveArgs = ReceiveSocket;
                userToken.UID = UID;
                userToken.ConnectSocket = ReceiveSocket.AcceptSocket;
                userToken.ActiveDateTime = DateTime.Now;
                userToken.ConnectDateTime = DateTime.Now;

                ReceiveSocket.UserToken = userToken;
                if (!connectsocketAsync.ConnectSocket.ReceiveAsync(ReceiveSocket))
                    Received(ReceiveSocket);
                IsConnected = true;
                RaiseOnConnected();
            }
        }

        public void DisConnect()
        {
            if (ReceiveSocket != null)
            {
                ReceiveSocket.Dispose();
            }
            if (clientSocket != null)
                clientSocket.Close();
            IsConnected = false;
        }

        private void SocketIO_Completed(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
                switch (e.LastOperation)
                {
                    case SocketAsyncOperation.Receive:
                        Received(e);
                        break;
                    case SocketAsyncOperation.Send:
                        Sended(e);
                        break;
                }
        }

        private void Received(SocketAsyncEventArgs e)
        {
            try
            {
                if ((e.SocketError == SocketError.Success) && e.AcceptSocket.Connected)
                {
                    var userToken = (IUserToken)e.UserToken;
                    var buffer = new byte[e.BytesTransferred];
                    Buffer.BlockCopy(userToken.ReceiveArgs.Buffer, e.Offset, buffer, 0, buffer.Length);
                    OnData?.BeginInvoke(this, buffer, null, null);
                    if (clientSocket.Connected)
                        if (!clientSocket.ReceiveAsync(ReceiveSocket))
                            Received(ReceiveSocket);
                }
                else
                {
                    RaiseOnError(new SocketException((int)SocketError.Shutdown), "已断开服务连接");
                }
            }
            catch (Exception ex)
            {
                RaiseOnError(ex, "ClientSocketOperation.Received");
            }
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="data"></param>
        public void SendAsync(byte[] data)
        {
            if ((clientSocket != null) && clientSocket.Connected && (data != null))
            {
                var sendSocket = new SocketAsyncEventArgs();
                sendSocket.SetBuffer(data, 0, data.Length);
                if (!clientSocket.SendAsync(sendSocket))
                    Sended(sendSocket);
            }
            else
            {
                RaiseOnError(new SocketException((int)SocketError.Shutdown), "发送消息失败");
            }
        }
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            this.SendAsync(System.Text.Encoding.UTF8.GetBytes(msg));
        }
       

        private void Sended(SocketAsyncEventArgs sendSocket)
        {
            sendSocket.Dispose();
        }

        #region event

        public virtual event OnConnectedHandler OnConnected;

        protected virtual void RaiseOnConnected()
        {
            OnConnected?.Invoke(this);
        }

        public virtual event OnCleintDataReceivedHandler OnData;

        public event OnClientErrorHandler OnError;

        public void RaiseOnError(Exception ex, string msg)
        {
            IsConnected = false;
            if ((OnError != null) && !string.IsNullOrEmpty(msg))
                OnError(ex, msg + Environment.NewLine + ex.Message + Environment.NewLine + ex.Source + ex.StackTrace);
        }

        #endregion
    }
}