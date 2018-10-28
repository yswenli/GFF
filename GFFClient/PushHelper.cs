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
using System.Net.Sockets;
using System.Text;
using GFF.Model.Entity;
using SAEA.MessageSocket;

namespace GFFClient
{
    public class PushHelper
    {
        public delegate void OnErrorHander(Exception ex, string msg);

        public delegate void OnMessageHanndle(string channelID, string msg);

        private static readonly object lockObj = new object();

        private string _channelID;

        private string _userName;

        /// <summary>
        ///     Tcp客户端
        /// </summary>
        public MessageClient Client { get; private set; }

        public void Start(string userName, string channelID)
        {
            _userName = userName;
            _channelID = channelID;

            Client = new MessageClient(_userName);
            Client.OnLogined += Client_OnLogined;
            Client.OnMessage += Client_OnMessage;
            Client.OnError += Client_OnError;
            Client.ConnectAsync();
        }

        public void Publish(string channelID, string value)
        {
            Client.PublishAsync(channelID, value);
        }


        public void SendFile(string channelID, string fileName, Action<string> callBack)
        {
            Client.HttpSendFileAsync(fileName, url => { callBack?.Invoke(url); });
        }
        
        private void Client_OnLogined(object sender, string msg)
        {
            Client.SubscribeAsync(_channelID);
        }

        private void Client_OnMessage(object sender, Message msg)
        {
            OnMessage?.Invoke(msg.Accepter, Encoding.UTF8.GetString(msg.Data));
        }

        private void Client_OnError(Exception ex, string msg)
        {
            lock (lockObj)
            {
                if (ex is SocketException)
                {
                    Client.ReConnectAsync();
                }
            }
        }

        public void Stop()
        {
            if (Client.IsConnected)
            {
                Client.UnsubscribeAsync(_channelID);
                Client.DisConnect();
            }
        }

        public event OnMessageHanndle OnMessage;

        public event OnErrorHander OnError;
    }
}