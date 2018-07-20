/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：8d231889-c143-48f2-b2d5-1fe668e19672
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Core.Interface
 * 类名称：IUserToken
 * 创建时间：2017/6/27 14:43:00
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace GFF.Core.Interface
{
    public interface IUserToken
    {
        string UID
        {
            get; set;
        }
        SocketAsyncEventArgs ReceiveArgs
        {
            get; set;
        }

        Socket ConnectSocket
        {
            get;
            set;
        }

        int MaxBufferSize
        {
            get;
        }
        DateTime ConnectDateTime
        {
            get; set;
        }
        DateTime ActiveDateTime
        {
            get; set;
        }

        void UnPackage(byte[] receiveData, Action<byte[]> action);
    }
}
