/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：0dc770cc-a197-4aea-8d9c-1b040848ce2d
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Core.Tcp
 * 类名称：TcpPackage
 * 创建时间：2016/12/5 10:11:20
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using GFF.Model.Enum;

namespace GFF.MS.Model
{
    /// <summary>
    /// tcp数据包
    /// |00|...|
    /// 总长度|类型|较验|内容
    /// |0000|0|0000|xxxxxxxxxxx
    /// </summary>
    public class TcpPackage : IDisposable
    {
        public int Length
        {
            get; set;
        }

        public Byte[] Data
        {
            get; set;
        }

        public MessageProtocalEnum Type
        {
            get; set;
        }

        public int Auth
        {
            get; set;
        }

        public byte[] Content
        {
            get; set;
        }
        /// <summary>
        /// tcp数据包
        /// 从字节数组中创建包
        /// </summary>
        /// <param name="contentData"></param>
        /// <param name="type"></param>
        /// <param name="auth"></param>
        /// <param name="offset"></param>
        public TcpPackage(byte[] contentData, MessageProtocalEnum type = MessageProtocalEnum.Heart, int auth = 0, int offset = 0)
        {
            byte[] nData = null;

            if (offset == 0)
            {
                nData = contentData;
            }
            else
            {
                nData = new byte[contentData.Length - offset];
                Buffer.BlockCopy(contentData, offset, nData, 0, nData.Length);
            }
            this.Length = nData.Length + 4 + 1 + 4;
            this.Data = new byte[this.Length];
            var len = BitConverter.GetBytes(this.Length);
            Buffer.BlockCopy(len, 0, this.Data, 0, 4);
            this.Auth = auth;
            var authArr = BitConverter.GetBytes(auth);
            Buffer.BlockCopy(authArr, 0, this.Data, 5, 4);
            this.Type = type;
            this.Data[4] = (byte)type;
            this.Content = nData;
            Buffer.BlockCopy(this.Content, 0, this.Data, 9, contentData.Length);
            Array.Clear(contentData, 0, contentData.Length);
        }

        public TcpPackage()
        {

        }

        /// <summary>
        /// 设置包的auth值
        /// </summary>
        /// <param name="auth"></param>
        public void SetAuth(int auth)
        {
            this.Auth = auth;
            var arr = BitConverter.GetBytes(auth);
            Buffer.BlockCopy(arr, 0, this.Data, 5, 4);
        }


        public void Dispose()
        {
            Array.Clear(this.Data, 0, this.Data.Length);
        }

        /// <summary>
        /// 将接收到的缓冲区转换成TcpPackage实例
        /// </summary>
        /// <param name="receiveData"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static TcpPackage Parse(byte[] receiveData, int offset = 0)
        {

            var package = new TcpPackage();
            package.Length = GetLength(receiveData, offset);
            try
            {
               
                var nData = new byte[package.Length];
                Buffer.BlockCopy(receiveData, offset, nData, 0, nData.Length);
                package.Data = nData;
                package.Type = GeTransportType(nData);
                package.Auth = GetAuth(nData);
                package.Content = GetContent(nData);
                return package;
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        /// <summary>
        /// 分析长度
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int GetLength(byte[] data, int offset = 0)
        {
            if (data != null && data.Length > offset + 9)
                return BitConverter.ToInt32(data, offset);
            return 0;
        }

        public static MessageProtocalEnum GeTransportType(byte[] data, int offset = 0)
        {
            if (data != null && data.Length >= offset + 4 + 1)
                return (MessageProtocalEnum)data[offset + 4];
            return MessageProtocalEnum.Heart;
        }

        public static int GetAuth(byte[] data, int offset = 0)
        {
            if (data != null && data.Length >= offset + 4 + 1 + 4)
                return BitConverter.ToInt32(data, offset + 5);
            return 0;
        }

        /// <summary>
        /// 获取数据内容
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static byte[] GetContent(byte[] data, int offset = 0)
        {
            var length = GetLength(data, offset);
            if (length > 9)
            {
                if (data.Length > offset + 4 + 1 + 4)
                {
                    var result = new byte[length - 9];
                    Buffer.BlockCopy(data, offset + 9, result, 0, result.Length);
                    return result;
                }
            }
            return null;
        }


        /// <summary>
        /// 生成心跳信息
        /// </summary>
        /// <returns></returns>
        public static byte[] GenerateHeart()
        {
            var data = new byte[1];
            data[0] = (byte)MessageProtocalEnum.Heart;
            using (var package = new TcpPackage(data))
            {
                return package.Data;
            }
        }

        /// <summary>
        /// 生成验证信息
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public static byte[] GenerateAuth(int auth)
        {
            var data1 = BitConverter.GetBytes(auth);
            var result = new byte[data1.Length + 1];
            result[0] = (byte)MessageProtocalEnum.Login;
            Buffer.BlockCopy(data1, 0, result, 1, data1.Length);
            data1 = null;
            using (var package = new TcpPackage(result))
            {
                return package.Data;
            }
        }

        /// <summary>
        /// 生成消息
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static byte[] GenerateMsg(int auth, string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                var data0 = BitConverter.GetBytes(auth);
                var data1 = Encoding.UTF8.GetBytes(msg);
                var result = new byte[data1.Length + 5];

                result[0] = (byte)MessageProtocalEnum.Message;
                Buffer.BlockCopy(data0, 0, result, 1, data0.Length);
                Buffer.BlockCopy(data1, 0, result, 5, data1.Length);

                Array.Clear(data0, 0, data0.Length);
                Array.Clear(data1, 0, data1.Length);
                data1 = data0 = null;

                using (var package = new TcpPackage(result))
                {
                    return package.Data;
                }
            }
            return null;
        }
        /// <summary>
        /// 生成文件
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="fileBytes"></param>
        /// <returns></returns>
        public static byte[] GenerateMsg(int auth, byte[] fileBytes)
        {
            var data0 = BitConverter.GetBytes(auth);
            var result = new byte[fileBytes.Length + 5];

            result[0] = (byte)MessageProtocalEnum.Message;
            Buffer.BlockCopy(data0, 0, result, 1, data0.Length);
            Buffer.BlockCopy(fileBytes, 0, result, 5, fileBytes.Length);
            fileBytes = data0 = null;
            using (var package = new TcpPackage(result))
            {
                return package.Data;
            }
        }

    }
}
