/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：da4b23ca-7e31-485f-b41a-e572ac4ae89b
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF
 * 类名称：ClientConfig
 * 创建时间：2016/11/4 17:47:06
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.IO;
using GFF.Helper;

namespace GFF
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
        public int Port { get; set; } = 6666;

        /// <summary>
        /// http服务器端口
        /// </summary>
        public int HttpPort { get; set; } = 8080; //http服务器端口
       

        public string FileUrl { get; set; }

        /// <summary>
        ///     解析命令初始缓存大小
        /// </summary>
        public int InitBufferSize { get; set; } = 1024*1; //解析命令初始缓存大小  

        /// <summary>
        ///     Socket超时设置为60秒
        /// </summary>
        public int SocketTimeOutMS { get; set; } = 60*1000; //Socket超时设置为60秒


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