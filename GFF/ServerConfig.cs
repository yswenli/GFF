/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：142e5106-13ca-418c-bcca-ee694f3658cb
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF
 * 类名称：ServerConfig
 * 创建时间：2016/11/4 17:44:06
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.IO;
using GFF.Helper;

namespace GFF
{
    public class ServerConfig
    {
        private static readonly string jsonFilePath = Environment.CurrentDirectory + "\\serverConfig.json";

        /// <summary>
        ///     服务器监听IP
        /// </summary>
        public string IP { get; set; } = "127.0.0.1"; //服务器监听IP

        /// <summary>
        ///     服务器监听端口
        /// </summary>
        public int Port { get; set; } = 6666; //服务器监听端口
        /// <summary>
        /// http服务器端口
        /// </summary>
        public int HttpPort { get; set; } = 8080; //http服务器端口       


        public int WSPort { get; set; } = 8082; //ws服务器端口       

        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024 * 1; //解析命令初始缓存大小  

        /// <summary>
        ///     Socket超时设置为60秒
        /// </summary>
        public int SocketTimeOutMS { get; set; } = 60 * 1000; //Socket超时设置为60秒

        /// <summary>
        ///     最大连接数
        /// </summary>
        public int MaxClientSize { get; set; } = 60000; //最大连接数     

        /// <summary>
        ///     服务器监听队列长度
        /// </summary>
        public int Backlog { get; set; } = 10000; //服务器监听队列长度   
        /// <summary>
        /// 控制服务器任务处理速度
        /// </summary>
        public int OperationThreads
        {
            get;
            set;
        } = 10;

        /// <summary>
        ///     服务器配置实例
        /// </summary>
        /// <returns></returns>
        public static ServerConfig Instance()
        {
            var config = new ServerConfig();
            try
            {
                var json = File.ReadAllText(jsonFilePath);
                return SerializeHelper.JsonDeserialize<ServerConfig>(json);
            }
            catch (FileNotFoundException fex)
            {
                Save(config);
            }
            return config;
        }

        /// <summary>
        ///     保存服务器配置
        /// </summary>
        /// <param name="config"></param>
        public static void Save(ServerConfig config)
        {
            var json = SerializeHelper.JsonSerialize(config);
            File.WriteAllText(jsonFilePath, json);
        }
    }
}