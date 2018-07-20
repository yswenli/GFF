/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：939c8cd9-d6e6-433b-b7e0-ed1fe37b5abf
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF
 * 类名称：MessageClient
 * 创建时间：2016/11/4 17:46:21
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GFF.Core.Handler;
using GFF.Core.Tcp;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;
using GFF.Model.Enum;
using GFF.MS.Model;

namespace GFF.MS
{
    /// <summary>
    ///     iocp客户端
    /// </summary>
    public class MessageClient : IDisposable
    {
        ClientSocket _client;

        /// <summary>
        ///     iocp客户端
        /// </summary>
        public MessageClient(string uid = "")
        {
            if (string.IsNullOrEmpty(ClientConfig.Instance().FileUrl))
                Url = "http://" + ClientConfig.Instance().IP + ":8080/";
            else
                Url = ClientConfig.Instance().FileUrl;

            _client = new ClientSocket(typeof(UserToken), ClientConfig.Instance().InitBufferSize, ClientConfig.Instance().SocketTimeOutMS, uid);

            _client.OnConnected += Client_OnConnected;

            _client.OnData += _client_OnMessage;

            _client.OnError += _client_OnError;
        }



        #region properties

        public DateTime ActiveTime
        {
            get;
            private set;
        } = DateTime.Now;

        public string Url
        {
            get; set;
        }

        public string UID
        {
            get
            {
                return _client.UID;
            }
        }

        public bool IsConnected
        {
            get
            {
                return _client.IsConnected;
            }
        }
        #endregion

        #region event

        public event OnLoginedHandler OnLogined;

        public event OnSubedHandler OnSubed;

        public event OnClientErrorHandler OnError;

        public event OnCleintMessageReceivedHandler OnMessage;

        public event OnCleintMessageReceivedHandler OnFile;

        #endregion

        #region 私有方法
        private void Client_OnConnected(object sender)
        {
            LoginAsync();
            HeartAsync();
        }

        private void _client_OnMessage(object sender, byte[] data)
        {
            ((UserToken)_client.ReceiveSocket.UserToken).UnPackage(data, (p) =>
            {
                try
                {
                    var msg = SerializeHelper.ProtolBufDeserialize<Message>(p.Content);
                    ActiveTime = DateTime.Now;
                    switch (msg.Protocal)
                    {
                        case (byte)MessageProtocalEnum.RLogin:
                            this.OnLogined?.Invoke(this, Encoding.UTF8.GetString(msg.Data));
                            break;
                        case (byte)MessageProtocalEnum.RSubscribe:
                            this.OnSubed(Encoding.UTF8.GetString(msg.Data));
                            break;
                        case (byte)MessageProtocalEnum.File:
                            OnFile?.Invoke(this, msg);
                            break;
                        case (byte)MessageProtocalEnum.Heart:
                            ActiveTime = DateTime.Now;
                            break;
                        default:
                            OnMessage?.Invoke(this, msg);
                            break;
                    }
                }
                catch
                {

                }
            });
        }

        private void _client_OnError(Exception ex, string msg)
        {
            this.OnError?.Invoke(ex, msg);
        }

        protected void LoginAsync()
        {
            this.SendMsgAsync(new Message
            {
                Accepter = _client.UID,
                Protocal = (byte)MessageProtocalEnum.Login,
                Sender = _client.UID
            });
        }

        private void LogoutAsync()
        {
            if (_client.IsConnected)
                this.SendMsgAsync(new Message
                {
                    Accepter = _client.UID,
                    Protocal = (byte)MessageProtocalEnum.Logout,
                    Sender = _client.UID
                });
        }

        protected void HeartAsync()
        {
            Task.Factory.StartNew(() =>
            {
                while (_client.IsConnected)
                    try
                    {
                        if (ActiveTime.AddSeconds(30) <= DateTime.Now)
                        {
                            this.SendMsgAsync(new Message
                            {
                                Protocal = (byte)MessageProtocalEnum.Heart,
                                Sender = _client.UID
                            });
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {
                        if (ex is SocketException)
                            ReConnectAsync();
                    }
            });
        }
        #endregion

        #region 公开方法
        /// <summary>
        ///     发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendMsgAsync(Message msg)
        {
            using (var p = new TcpPackage(SerializeHelper.ProtolBufSerialize(msg)))
            {
                var buffer = p.Data;

                _client.SendAsync(buffer);
            }
        }



        public void DisConnect()
        {
            _client.DisConnect();
        }


        /// <summary>
        ///     关闭连接
        /// </summary>
        public void Dispose()
        {
            LogoutAsync();
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
        public void ReConnectAsync()
        {
            LogoutAsync();
            _client.DisConnect();
            Thread.Sleep(3000);
            ConnectAsync();
        }




        #region 私信模式

        /// <summary>
        ///     发送私信
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="msg"></param>
        public void SendPrivateMsg(string uid, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Message message = new Message
                {
                    Accepter = uid,
                    Protocal = (byte)MessageProtocalEnum.PrivateMsg,
                    Data = Encoding.UTF8.GetBytes(msg),
                    Sender = _client.UID
                };
                this.SendMsgAsync(message);
                ActiveTime = DateTime.Now;
            }
        }

        #endregion


        #region 发布订阅模式

        /// <summary>
        ///     发布消息
        /// </summary>
        /// <param name="channelID"></param>
        /// <param name="msg"></param>
        public void PublishAsync(string channelID, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                Message message = new Message
                {
                    Accepter = channelID,
                    Protocal = (byte)MessageProtocalEnum.Message,
                    Data = Encoding.UTF8.GetBytes(msg),
                    Sender = _client.UID
                };
                this.SendMsgAsync(message);
                ActiveTime = DateTime.Now;
            }
        }

        /// <summary>
        ///     订阅消息
        /// </summary>
        /// <param name="channelID"></param>
        public void SubscribeAsync(string channelID)
        {
            if (!string.IsNullOrEmpty(channelID))
            {
                var message = new Message
                {
                    Protocal = (byte)MessageProtocalEnum.Subscribe,
                    Accepter = channelID,
                    Sender = _client.UID
                };
                this.SendMsgAsync(message);
                ActiveTime = DateTime.Now;
            }
        }

        /// <summary>
        ///     取消订阅
        /// </summary>
        /// <param name="channelID"></param>
        public void UnsubscribeAsync(string channelID)
        {
            if (!string.IsNullOrEmpty(channelID))
            {
                var message = new Message
                {
                    Protocal = (byte)MessageProtocalEnum.Unsubscribe,
                    Accepter = channelID,
                    Sender = _client.UID
                };
                this.SendMsgAsync(message);
            }
        }

        #endregion

        #region SendFile

        public void HttpSendFileAsync(string url, string fileName, Action<string> callBack)
        {
            Task.Factory.StartNew(() =>
            {
                var fileUrl = string.Empty;
                try
                {
                    using (var webClient = new WebClientUtil { Encoding = Encoding.UTF8 })
                    {
                        fileUrl = Encoding.UTF8.GetString(webClient.UploadFile(url, fileName));
                        callBack?.Invoke(fileUrl);
                    }
                }
                catch (Exception ex)
                {
                    _client.RaiseOnError(ex, "发送文件失败！");
                }
            });
        }

        public void HttpSendFileAsync(string fileName, Action<string> callBack)
        {
            Task.Factory.StartNew(() =>
            {
                var fileUrl = string.Empty;
                try
                {
                    using (var webClient = new WebClientUtil())
                    {
                        fileUrl = Encoding.UTF8.GetString(webClient.UploadFile(Url, fileName));
                        callBack?.Invoke(fileUrl);
                    }
                }
                catch (Exception ex)
                {
                    _client.RaiseOnError(ex, "发送文件失败！");
                }
            });
        }

        public void SendFileAsync(string uid, string fileName)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var message = new Message
                    {
                        Protocal = (byte)MessageProtocalEnum.File,
                        Accepter = uid,
                        Sender = _client.UID
                    };
                    message.Data = File.ReadAllBytes(fileName);
                    this.SendMsgAsync(message);
                    ActiveTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _client.RaiseOnError(ex, "发送文件失败！");
                }
            });
        }
        public void SendFileAsync(string uid, byte[] file)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var message = new Message
                    {
                        Protocal = (byte)MessageProtocalEnum.File,
                        Accepter = uid,
                        Sender = _client.UID
                    };
                    message.Data = file;
                    this.SendMsgAsync(message);
                    ActiveTime = DateTime.Now;
                }
                catch (Exception ex)
                {
                    _client.RaiseOnError(ex, "发送文件失败！");
                }
            });
        }

        #endregion

        #endregion

    }
}