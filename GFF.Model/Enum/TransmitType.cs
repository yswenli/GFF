/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：083ef75b-1c26-4b53-810a-fcc4359d79f2
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Model.Enum
 * 类名称：TransmitType
 * 创建时间：2016/11/8 16:06:34
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFF.Model.Enum
{
    /// <summary>
    ///  UDP传输类型
    /// </summary>
    public enum TransmitType
    {
        /// <summary>
        /// 获得远程主机信息
        /// </summary>
        getRemoteEP = 126,
        /// <summary>
        /// 传输文件数据包
        /// </summary>
        getFilePackage = 235,
        /// <summary>
        /// 音频包
        /// </summary>
        Audio = 237,
        /// <summary>
        /// 视频包
        /// </summary>
        Video = 239,
        /// <summary>
        ///传输命令 
        /// </summary>
        Cmd = 241,
        /// <summary>
        /// 打洞数据包
        /// </summary>
        Penetrate = 114,
        /// <summary>
        /// 传输完成
        /// </summary>
        over = 118,
    }
}
