/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：e7459e0c-fb36-4e0b-a14c-620beb231c4d
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS
 * 类名称：WSToken
 * 创建时间：2017/6/27 14:40:48
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using GFF.Core.Interface;
using GFF.Core.Tcp.Model;

namespace GFF.WS.Model
{
    public class WSUserToken : IUserToken
    {
        public string UID
        {
            get; set;
        }
        public SocketAsyncEventArgs ReceiveArgs
        {
            get; set;
        } = new SocketAsyncEventArgs();


        Socket connectSocket;
        public Socket ConnectSocket
        {
            get
            {
                return connectSocket;
            }
            set
            {
                connectSocket = value;
                ReceiveArgs.AcceptSocket = connectSocket;
            }
        }

        public int MaxBufferSize
        {
            get;
        }
        public DateTime ConnectDateTime
        {
            get; set;
        }
        public DateTime ActiveDateTime
        {
            get; set;
        }

        /// <summary>
        /// websocket中握手标记
        /// </summary>
        public bool HandShaked
        {
            get; set;
        } = false;


        private static object locker = new object();


        byte[] _buffer = new byte[1024];

        int _offset = 0;

        public void UnPackage(byte[] recBytes, Action<byte[]> action)
        {
            lock (locker)
            {
                if (_buffer.Length - _offset < recBytes.Length)
                {
                    var cache = new byte[(int)Math.Ceiling((recBytes.Length + _offset) / 1024f) * 1024];
                    Buffer.BlockCopy(_buffer, 0, cache, 0, _offset);
                    _buffer = cache;
                }
                Buffer.BlockCopy(recBytes, 0, _buffer, _offset, recBytes.Length);
                _offset += recBytes.Length;

                foreach (var data in AnalyzeCacheData())
                {
                    if (data != null)
                    {
                        action?.BeginInvoke(data, null, null);
                    }
                }
            }
        }


        /// <summary>
        /// 解析客户端发送来的数据
        /// </summary>
        private IEnumerable<byte[]> AnalyzeCacheData()
        {
            var offset = 0;
            while (true)
            {
                if (_offset - offset < 2)
                {
                    break;
                }
                bool fin = (_buffer[offset] & 0x80) == 0x80; // 1bit，1表示最后一帧  
                if (!fin)
                {
                    break;// 超过一帧暂不处理 
                }
                bool mask = (_buffer[offset + 1] & 0x80) == 0x80; // 是否包含掩码  
                int payloadLen = _buffer[offset + 1] & 0x7F; // 数据长度  
                byte[] payloadData = null;
                if (mask)
                {
                    var masks = new byte[4];
                    if (payloadLen == 126)
                    {
                        if (_offset - offset < 8)
                            break;
                        var len = (ushort)(_buffer[offset + 2] << 8 | _buffer[offset + 3]);
                        if (_offset - offset < len + 8)
                            break;
                        Array.Copy(_buffer, offset + 4, masks, 0, 4);
                        payloadData = new byte[len];
                        Array.Copy(_buffer, offset + 8, payloadData, 0, len);
                        offset += 8 + len;
                        DeMask(payloadData, 0, len, masks);
                    }
                    else
                    {
                        if (_offset - offset < payloadLen + 6)
                            break;
                        Array.Copy(_buffer, offset + 2, masks, 0, 4);
                        payloadData = new byte[payloadLen];
                        Array.Copy(_buffer, offset + 6, payloadData, 0, payloadLen);
                        offset += 6 + payloadLen;
                        DeMask(payloadData, 0, payloadLen, masks);
                    }
                }
                else
                {
                    if (payloadLen == 126)
                    {
                        if (_offset - offset < 4)
                            break;
                        var len = (ushort)(_buffer[offset + 2] << 8 | _buffer[offset + 3]);
                        if (_offset - offset < len + 4)
                            break;
                        payloadData = new byte[len];
                        Array.Copy(_buffer, offset + 4, payloadData, 0, len);
                        offset += 4 + len;
                    }                    
                    else
                    {
                        if (_offset - offset < payloadLen + 2)
                            break;
                        payloadData = new byte[payloadLen];
                        Array.Copy(_buffer, offset + 2, payloadData, 0, payloadLen);
                        offset += 2 + payloadLen;
                    }
                }
                yield return payloadData;
            }
            _offset -= offset;
            Buffer.BlockCopy(_buffer, offset, _buffer, 0, _offset);
        }

        private void DeMask(byte[] buffer, int offset, int length, byte[] masks)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[offset + i] = (byte)(buffer[offset + i] ^ masks[i % 4]);
            }
        }
    }
}
