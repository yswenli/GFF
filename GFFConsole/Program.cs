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
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GFF;
using GFF.Core.Interface;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;
using GFF.Model.Enum;
using GFF.MS;
using GFF.MS.Model;
using GFF.WS;

namespace GFFConsole
{
    internal class Program
    {
        private static MessageServer server;


        private static readonly string uid1 = "张姗姗" + Guid.NewGuid().ToString("N");

        private static readonly string uid2 = "李思思" + Guid.NewGuid().ToString("N");

        private static readonly string testString =
            "这次国行版 Surface Book 一共推出了 4 个版本，依据配备的处理器和存储空间大小不同，国行价格也分别从 11088 元到 20088 元不等。动点科技拿到的是 Core i5 处理器、8 GB 内存、128 GB 存储的版本这次国行版 Surface Book 一共推出了 4 个版本，依据配备的处理器和存储空间大小不同，国行价格也分别从 11088 元到 20088 元不等。";


        private static void Main(string[] args)
        {
            Thread serverThread = null;

            Console.WriteLine("是否启动服务器？Y/N");

            var result = Console.ReadLine().ToUpper();

            if (result != "N")
            {
                serverThread = new Thread(() =>
                {
                    ConsoleHelper.WriteLine("正在初始化服务器...", ConsoleColor.Green);
                    server = new MessageServer();
                    server.OnAccepted += Server_OnAccepted;
                    server.OnErrored += Server_OnErrored;
                    //server.OnReceived += Server_OnReceived;
                    server.OnUnAccepted += Server_OnUnAccepted;
                    ConsoleHelper.WriteLine("服务器初始化完毕...", ConsoleColor.Green);
                    ConsoleHelper.WriteLine("正在启动服务器...", ConsoleColor.Green);
                    server.Start();

                    ConsoleHelper.WriteLine("正在启动wsserver...");
                    WSServer wsserver = new WSServer();
                    var wsurl = "ws://172.31.56.106:" + ServerConfig.Instance().WSPort;
                    wsserver.Start(wsurl);
                    ConsoleHelper.WriteLine("wsserver正在运行...");

                    ConsoleHelper.WriteLine("服务器启动完毕...", ConsoleColor.Green);
                });
                serverThread.Start();
                Console.Title = "GFFServer";
                Console.ReadLine();
                //return;
            }

            Console.Title = "GFFClient";

            ConsoleHelper.WriteLine("私信测试开始...");
            var client1 = new MessageClient(uid1);
            client1.OnMessage += Client_OnMessage;
            client1.OnLogined += Client1_OnLogined;
            client1.OnError += Client_OnError;
            client1.ConnectAsync();

            Console.ReadLine();

            var client2 = new MessageClient(uid2);
            client2.OnMessage += Client_OnMessage;
            client2.OnError += Client_OnError;
            client2.OnLogined += Client2_OnLogined;
            client2.ConnectAsync();
            Console.ReadLine();


            var channelID = "all";
            ConsoleHelper.WriteLine("频道信息测试开始...");

            var client3 = new MessageClient();
            client3.OnMessage += Client_OnMessage;
            client3.OnError += Client_OnError;
            client3.OnLogined += Client3_OnLogined;
            client3.ConnectAsync();
            Console.ReadLine();


            ConsoleHelper.WriteLine("订阅端测试开始...");
            Parallel.For(0, 10000, (i) =>
            {
                var ss = new MessageClient();
                ss.OnLogined += Ss_OnLogined;
                ss.ConnectAsync();
            });
            Console.ReadLine();
            ConsoleHelper.WriteLine("点击回车结束测试...");
            //
            Console.ReadLine();
            if (serverThread != null)
                serverThread.Abort();
        }

        #region server events

        private static void Server_OnAccepted(int num, IUserToken userToken)
        {
            ConsoleHelper.WriteInfo(string.Format("客户端已{0}连接，当前连接数共记：{1}", userToken.UID, num));
        }

        private static void Server_OnUnAccepted(int num, IUserToken userToken)
        {
            ConsoleHelper.WriteInfo(string.Format("客户端{0}已断开连接，当前连接数共记：{1}", userToken.UID, num));
        }

        private static void Server_OnReceived(UserToken userToken, Message msg)
        {
            var protocal = string.Empty;
            var protocalName = "频道：";
            switch (msg.Protocal)
            {
                case (byte)MessageProtocalEnum.Heart:
                    protocal = MessageProtocalEnum.Heart.ToString();
                    protocalName = "心跳";
                    break;
                case (byte)MessageProtocalEnum.Login:
                    protocal = MessageProtocalEnum.Login.ToString();
                    protocalName = "登录：";
                    break;
                case (byte)MessageProtocalEnum.Logout:
                    protocal = MessageProtocalEnum.Logout.ToString();
                    protocalName = "登出：";
                    break;
                case (byte)MessageProtocalEnum.PrivateMsg:
                    protocal = MessageProtocalEnum.PrivateMsg.ToString();
                    protocalName = "私信：";
                    break;
                case (byte)MessageProtocalEnum.Subscribe:
                    protocal = MessageProtocalEnum.Subscribe.ToString();
                    protocalName = "订阅：";
                    break;
                case (byte)MessageProtocalEnum.Message:
                    protocal = MessageProtocalEnum.Message.ToString();
                    protocalName = "消息：";
                    break;
            }


            if (msg.Protocal == (byte)GFF.Model.Enum.MessageProtocalEnum.File)
            {
                ConsoleHelper.WriteLine(
                string.Format("收到客户端信息；协议：{0}，频道：{1}，内容：文件", protocalName, msg.Accepter, ConsoleColor.Green, false));
            }
            else
            {
                ConsoleHelper.WriteLine(
                string.Format("收到客户端信息；协议：{0}，频道：{1}，内容：{2}", protocalName, msg.Accepter,
                 msg.Data == null ? "" : Encoding.UTF8.GetString(msg.Data)), ConsoleColor.Green, false);
            }
        }

        private static void Server_OnErrored(Exception ex, params object[] args)
        {
            ConsoleHelper.WriteErr(ex, args);
        }

        #endregion

        #region client events

        private static void Client1_OnLogined(object sender, string msg)
        {
            var client1 = sender as MessageClient;

            client1.SubscribeAsync("all");

            FunHelper.WhileAsync(() =>
            {
                client1.SendPrivateMsg(uid2, "你好思思，我是姗姗");
            }, 1000, true);
        }

        private static void Client2_OnLogined(object sender, string msg)
        {
            var client2 = sender as MessageClient;

            client2.SubscribeAsync("all");

            FunHelper.WhileAsync(() =>
            {
                client2.SendPrivateMsg(uid1, "你好姗姗，我是思思");
            }, 1000, true);
        }

        private static void Client3_OnLogined(object sender, string msg)
        {
            Task.Factory.StartNew(() =>
            {
                var client3 = sender as MessageClient;
                client3.SubscribeAsync("all");
                FunHelper.WhileAsync(() =>
                {
                    client3.PublishAsync("all", msg);
                }, 3000);
            });
        }

        private static void Ss_OnLogined(object sender, string msg)
        {
            Task.Factory.StartNew(() =>
            {
                var ss = sender as MessageClient;
                ss.SubscribeAsync("all");
            });
        }

        private static void Client_OnMessage(object sender, Message msg)
        {
            var client = sender as MessageClient;
            var protocal = string.Empty;
            var protocalName = "频道：";
            switch (msg.Protocal)
            {
                case (byte)MessageProtocalEnum.Heart:
                    protocal = MessageProtocalEnum.Heart.ToString();
                    protocalName = "";
                    break;
                case (byte)MessageProtocalEnum.Login:
                    protocal = MessageProtocalEnum.Login.ToString();
                    protocalName = "用户：";
                    break;
                case (byte)MessageProtocalEnum.Logout:
                    protocal = MessageProtocalEnum.Logout.ToString();
                    protocalName = "用户：";
                    break;
                case (byte)MessageProtocalEnum.PrivateMsg:
                    protocal = MessageProtocalEnum.PrivateMsg.ToString();
                    protocalName = "用户：";
                    break;
                case (byte)MessageProtocalEnum.Subscribe:
                    protocal = MessageProtocalEnum.Subscribe.ToString();
                    protocalName = "频道：";
                    break;
                case (byte)MessageProtocalEnum.Message:
                    protocal = MessageProtocalEnum.Message.ToString();
                    protocalName = "频道：";
                    break;
            }
            ConsoleHelper.WriteLine(
                string.Format("客户端【{0}】收到服务器回复；协议：{1}，{2}，接收者：{3}，内容：{4}，发送者：{5},发送时间：{6}", client.UID, protocal,
                    protocalName, msg.Accepter, msg.Data == null ? "" : Encoding.UTF8.GetString(msg.Data), msg.Sender,
                    new DateTime(msg.SendTick).ToString("yyyy-mm-dd HH:mm:ss.fff")), ConsoleColor.Yellow, false);
        }
        private static void Client_OnError(Exception ex, string msg)
        {
            ConsoleHelper.WriteLine(msg + ",error:" + ex.Message);
        }
        #endregion
    }
}