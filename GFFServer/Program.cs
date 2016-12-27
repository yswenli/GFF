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
using System.Text;
using GFF;
using GFF.Core.Tcp.Model;
using GFF.Helper;
using GFF.Model.Entity;

namespace GFFServer
{
    internal class Program
    {
        private static MessageServer server;

        private static void Main(string[] args)
        {
            ConsoleHelper.WriteLine("正在初始化服务器...", ConsoleColor.Green);
            server = new MessageServer();
            server.OnAccepted += Server_OnAccepted;
            server.OnErrored += Server_OnErrored;
            server.OnReceived += Server_OnReceived;
            server.OnUnAccepted += Server_OnUnAccepted;
            ConsoleHelper.WriteLine("服务器初始化完毕...", ConsoleColor.Green);
            ConsoleHelper.WriteLine("正在启动服务器...", ConsoleColor.Green);
            server.Start();
            ConsoleHelper.WriteLine("服务器启动完毕...", ConsoleColor.Green);
            ConsoleHelper.WriteLine("点击回车，结束服务");
            Console.ReadLine();
        }

        private static void Server_OnAccepted(int num, UserToken userToken)
        {
            ConsoleHelper.WriteInfo(string.Format("客户端{0}已连接，当前连接数共记：{1}", userToken.UID, num));
        }

        private static void Server_OnUnAccepted(int num, UserToken userToken)
        {
            ConsoleHelper.WriteInfo(string.Format("客户端{0}已断开连接，当前连接数共记：{1}", userToken.UID, num));
        }

        private static void Server_OnReceived(UserToken userToken, Message msg)
        {
            ConsoleHelper.WriteLine(
                string.Format("收到客户端信息；协议：{0}，频道：{1}，内容：{2}", msg.Protocal, msg.Accepter,
                    msg.Data == null ? "" : Encoding.UTF8.GetString(msg.Data)), ConsoleColor.Green, false);
        }

        private static void Server_OnErrored(Exception ex, params object[] args)
        {
            ConsoleHelper.WriteErr(ex, args);
        }
    }
}