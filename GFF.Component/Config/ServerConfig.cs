using GFF.Helper;
using System;
using System.IO;

namespace GFF.Component.Config
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
        public int Port { get; set; } = 39654; //服务器监听端口  

        
        public int FilePort { get; set; } = 39655;

        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024 * 10; //解析命令初始缓存大小  

        /// <summary>
        ///     最大连接数
        /// </summary>
        public int MaxClientSize { get; set; } = 10000; //最大连接数     


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
