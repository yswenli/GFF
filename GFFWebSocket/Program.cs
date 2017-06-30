using System;
using System.Text;
using GFF.Core.Interface;
using GFF.Helper;
using GFF.WS;

namespace GFFWSConsole
{
    class Program
    {

        static string url = "ws://localhost:8082/";

        static void Main(string[] args)
        {
            Console.Title = "websocket test";

            ConsoleHelper.WriteLine("输入Y启动WS服务器");

            if (Console.ReadLine().ToUpper() == "Y")
            {
                ServerInit();
            }

            ConsoleHelper.WriteLine("回车启动客户端...");
            Console.ReadLine();

            for (int i = 0; i < 50000; i++)
            {
                if (i == 0)
                    ClientInit(true);
                ClientInit();
            }



            Console.ReadLine();
        }


        #region wsserver
        private static void ServerInit()
        {
            ConsoleHelper.WriteLine("正在启动wsserver...");
            WSServer server = new WSServer();
            server.OnConnected += Server_OnConnected;
            server.OnErrored += Server_OnErrored;
            server.OnUnAccepted += Server_OnUnAccepted;
            server.OnSocketReceived += Server_OnSocketReceived;
            server.Start(url);
            ConsoleHelper.WriteLine("wsserver正在运行...");
        }

        private static void Server_OnSocketReceived(IUserToken userToken, byte[] args)
        {
            ConsoleHelper.WriteLine("服务器收到消息：" + Encoding.UTF8.GetString(args));
        }

        private static void Server_OnErrored(Exception ex, params object[] args)
        {
            ConsoleHelper.WriteErr(ex);
        }

        private static void Server_OnConnected(int num, IUserToken userToken)
        {
            ConsoleHelper.WriteLine("当前服务器连接数：" + num);
        }

        private static void Server_OnUnAccepted(int num, IUserToken userToken)
        {
            ConsoleHelper.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 客户" + userToken.UID + "已断开，当前服务器连接数：" + num);
        }
        #endregion

        #region client
        public static void ClientInit(bool status = false)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 客户端测试");

            WSClient client = new WSClient();
            if (!status)
                client.OnConnected += Client_OnConnected;
            else
                client.OnConnected += Client_OnConnected1;
            client.OnReceived += Client_OnReceived;
            client.OnReceivedMessage += Client_OnReceivedMessage;
            client.ConnectAsync(url);
        }

        private static void Client_OnReceivedMessage(GFF.WS.Model.WSMessage obj)
        {
            ConsoleHelper.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 当前客户端收到消息：" + obj.Content);
        }

        private static void Client_OnReceived(byte[] obj)
        {
            //ConsoleHelper.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 当前客户端收到消息：" + Encoding.UTF8.GetString(obj));
        }
        private static void Client_OnConnected(WSClient client)
        {
            ConsoleHelper.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 当前客户端已连上");

        }
        private static void Client_OnConnected1(WSClient client)
        {
            ConsoleHelper.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " 当前客户端已连上");

            FunHelper.WhileAsync(() =>
            {
                //client.SendAsync(longStr);
                client.SendAsync(new GFF.WS.Model.WSMessage()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    Content = longStr,
                    Sender = "CS",
                    time = DateTime.Now.Ticks.ToString()
                });

            }, 1000);

        }

        #endregion

        static string shortStr = "近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品";

        static string longStr = "近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。近日，创新工场完成对 F5 未来商店的 3000 万元 A++ 轮融资，融资将用于产品、供应链的进一步打磨和快速布店。此前 F5 曾获得创大资本 200 万元的天使轮融资，以及 TCL 创投的 1000 万元 Pre-A 轮融资。";
    }
}
