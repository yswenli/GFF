/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：20da4241-0bdc-4a06-8793-6d0889c31f95
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：MCITest


 * 创建年份：2015
 * 创建时间：2015-12-02 11:15:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Runtime.Serialization;
using GFF.Model.InterFace;
using ProtoBuf;

namespace GFF.Model.Entity
{
    [DataContract]
    [ProtoContract]
    public class Message : IMessage, IDisposable
    {
        /// <summary>
        ///     频道ID或私信ID
        /// </summary>
        [DataMember]
        [ProtoMember(1, IsRequired = false)]
        public string Accepter
        {
            get; set;
        }

        [DataMember]
        [ProtoMember(2, IsRequired = true)]
        public byte Protocal
        {
            get; set;
        }

        [DataMember]
        [ProtoMember(3, IsRequired = true)]
        public byte[] Data
        {
            get; set;
        }

        /// <summary>
        ///     发送者id
        /// </summary>
        [DataMember]
        [ProtoMember(4, IsRequired = true)]
        public string Sender
        {
            get; set;
        }

        /// <summary>
        ///     发送时间
        /// </summary>
        [DataMember]
        [ProtoMember(5, IsRequired = true)]
        public long SendTick
        {
            get; set;
        }

        public void Dispose()
        {
            this.Accepter = null;
            this.Protocal = 0;
            if (Data != null)
                Array.Clear(this.Data, 0, this.Data.Length);
            this.Sender = null;
            this.SendTick = 0;
        }

    }
}