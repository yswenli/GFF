/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：433fa350-1411-477b-8c5f-f315fb2bce1d
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Core.Interface
 * 类名称：IServer
 * 创建时间：2017/6/26 10:52:55
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GFF.Core.Handler;
using GFF.Core.Tcp.Model;
using GFF.Model.Entity;

namespace GFF.Core.Interface
{
    public interface IServer
    {
        event OnServerErrorHandler OnErrored;

        /// <summary>
        /// 启动服务
        /// </summary>
        void Start();

        /// <summary>
        /// 停止服务器
        /// </summary>
        void Stop();

        /// <summary>
        /// 是否启动
        /// </summary>
        bool IsStarted
        {
            get;
        }

        /// <summary>
        /// 在线客户端数
        /// </summary>
        int OnlineNums
        {
            get;
        }
    }
}
