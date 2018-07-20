/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：ee7abfe1-48ef-4b07-bdd4-5caddf9b9a02
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS
 * 类名称：WSFrame
 * 创建时间：2017/6/26 11:12:45
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;


namespace GFF.WS
{
    /// <summary>
    /// Websocket frame类
    /// </summary>
    internal class WSFrame
    {
        private static string CrLf = "\r\n";

        #region server      

        #region 打包请求连接数据
        /// <summary>
        /// 打包请求连接数据
        /// </summary>
        /// <param name="handShakeBytes"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static byte[] PackageHandShakeData(byte[] handShakeBytes, int length)
        {
            string handShakeText = Encoding.UTF8.GetString(handShakeBytes, 0, length);
            string key = string.Empty;
            Regex reg = new Regex(@"Sec\-WebSocket\-Key:(.*?)" + CrLf);
            Match m = reg.Match(handShakeText);
            if (m.Value != "")
            {
                key = Regex.Replace(m.Value, @"Sec\-WebSocket\-Key:(.*?)" + CrLf, "$1").Trim();
            }
            byte[] secKeyBytes = SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(key + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11"));
            string secKey = Convert.ToBase64String(secKeyBytes);
            var responseBuilder = new StringBuilder();
            responseBuilder.Append("HTTP/1.1 101 Switching Protocols" + CrLf);
            responseBuilder.Append("Upgrade: websocket" + CrLf);
            responseBuilder.Append("Connection: Upgrade" + CrLf);
            responseBuilder.Append("Sec-WebSocket-Accept: " + secKey + CrLf + CrLf);
            //responseBuilder.Append("Sec-WebSocket-Protocol: wenli.chat " + CrLf + CrLf);
            return Encoding.UTF8.GetBytes(responseBuilder.ToString());
        }
        #endregion



        #endregion

        #region client
        private static string CreateBase64Key()
        {
            var src = new byte[16];
            Ext.RandomNumber.GetBytes(src);
            return Convert.ToBase64String(src);
        }

        /// <summary>
        /// 打包请求连接数据
        /// </summary>
        /// <param name="serverIP"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public static string PackageClientHandShark(string serverIP, int port)
        {
            var sb = new StringBuilder(64);
            sb.AppendFormat("{0} ws:{1}:{2} HTTP/{3}{4}", "GET", serverIP, port, "1.1", CrLf);
            sb.AppendFormat("{0}: {1}{2}", "Upgrade", "websocket", CrLf);
            sb.AppendFormat("{0}: {1}{2}", "Connection", "Upgrade", CrLf);
            sb.AppendFormat("{0}: {1}{2}", "Sec-WebSocket-Key", CreateBase64Key(), CrLf);
            sb.AppendFormat("{0}: {1}{2}", "Sec-WebSocket-Protocol", "wenli.chat", CrLf);
            sb.AppendFormat("{0}: {1}{2}", "Sec-WebSocket-Version", "13", CrLf);
            sb.Append(CrLf);
            return sb.ToString();
        }
        #endregion


        #region 封包

        public static byte[] PackageData(string message)
        {
            return PackageData(Encoding.UTF8.GetBytes(message));
        }
        public static byte[] PackageData(byte[] message)
        {
            int _payloadLength;
            byte[] _extPayloadLength;

            ulong len = (ulong)message.Length;
            if (len < 126)
            {
                _payloadLength = (byte)len;
                _extPayloadLength = new byte[0];
            }
            else if (len < 0x010000)
            {
                _payloadLength = (byte)126;
                _extPayloadLength = ((ushort)len).InternalToByteArray(ByteOrder.Big);
            }
            else
            {
                _payloadLength = (byte)127;
                _extPayloadLength = len.InternalToByteArray(ByteOrder.Big);
            }

            using (var buff = new MemoryStream())
            {
                var header = (int)0x1;
                header = (header << 1) + (int)0x0;
                header = (header << 1) + (int)0x0;
                header = (header << 1) + (int)0x0;
                header = (header << 4) + (int)0x2;
                header = (header << 1) + (int)0x0;
                header = (header << 7) + (int)_payloadLength;
                buff.Write(((ushort)header).InternalToByteArray(ByteOrder.Big), 0, 2);

                if (_payloadLength > 125)
                    buff.Write(_extPayloadLength, 0, _payloadLength == 126 ? 2 : 8);

                if (_payloadLength > 0)
                {
                    buff.Write(message, 0, message.Length);
                }

                buff.Close();
                return buff.ToArray();
            }
        }
        #endregion

    }
}
