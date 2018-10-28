using GFF.Helper;
using System;
using System.IO;

namespace GFF.Component.Config
{
    /// <summary>
    ///     客户端配置类
    /// </summary>
    public class ClientConfig
    {
        /// <summary>
        ///     服务器监听IP
        /// </summary>
        public string IP { get; set; } = "127.0.0.1";

        /// <summary>
        ///     服务器监听端口
        /// </summary>
        public int Port { get; set; } = 39654;

        /// <summary>
        ///     文件服务器
        /// </summary>
        public string Url { get; set; } = "http://127.0.0.1:39655/File/"; //文件服务器


        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024 * 10; //解析命令初始缓存大小  


        public static ClientConfig Instance()
        {
            var config = new ClientConfig();
            try
            {
                var json = File.ReadAllText(Environment.CurrentDirectory + "\\config.json");
                return SerializeHelper.JsonDeserialize<ClientConfig>(json);
            }
            catch (FileNotFoundException fex)
            {
                Save(config);
            }
            return config;
        }

        public static void Save(ClientConfig config)
        {
            var json = SerializeHelper.JsonSerialize(config);
            File.WriteAllText(Environment.CurrentDirectory + "\\config.json", json);
        }
    }
}
