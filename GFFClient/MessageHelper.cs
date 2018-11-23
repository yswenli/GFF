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

using GFF.Component.Config;
using SAEA.MessageSocket;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GFFClient
{
    public class MessageHelper
    {
        public delegate void OnErrorHander(Exception ex, string msg);

        public delegate void OnMessageHanndle(string channelID, string msg);

        private static readonly object lockObj = new object();

        private string _channelID;

        private string _userName;

        ClientConfig clientConfig;

        public MessageHelper()
        {
            clientConfig = ClientConfig.Instance();
        }

        /// <summary>
        ///     Tcp客户端
        /// </summary>
        public MessageClient Client { get; private set; }

        public void Start(string userName, string channelID)
        {
            _userName = userName;
            _channelID = channelID;

            Client = new MessageClient(10240, clientConfig.IP, clientConfig.Port);
            Client.OnChannelMessage += Client_OnChannelMessage;
            Client.OnPrivateMessage += Client_OnPrivateMessage;
            Client.OnError += Client_OnError;
            Client.Connect();
            Client.Login();
            Client.Subscribe(channelID);
        }

        private void Client_OnError(string ID, Exception ex)
        {
            OnError.Invoke(ex, ex.Message);
        }

        private void Client_OnChannelMessage(SAEA.MessageSocket.Model.Business.ChannelMessage msg)
        {
            OnMessage?.Invoke(_channelID, msg.Content);
        }

        private void Client_OnPrivateMessage(SAEA.MessageSocket.Model.Business.PrivateMessage msg)
        {
            OnMessage?.Invoke(msg.Receiver, msg.Content);
        }

        public void Publish(string channelID, string value)
        {
            Client.SendChannelMsg(channelID, value);
        }


        public void SendFile(string channelID, string fileName, Action<string> callBack)
        {
            HttpSendFileAsync(fileName, url => { callBack?.Invoke(url); });
        }


        public void HttpSendFileAsync(string fileName, Action<string> callBack)
        {
            Task.Run(() =>
            {
                using (WebClient webClient = new WebClient())
                {
                    var url = clientConfig.Url + Encoding.UTF8.GetString(webClient.UploadFile(clientConfig.Url + "Upload", fileName));
                    callBack.Invoke(url);
                }
            });
        }

        public void Stop()
        {
            try
            {
                Client.Dispose();
            }
            catch { }
        }

        public event OnMessageHanndle OnMessage;

        public event OnErrorHander OnError;
    }
}