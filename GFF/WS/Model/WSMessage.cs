/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：2562eeeb-df19-49d0-a400-143d3733a040
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.WS.Model
 * 类名称：WSMessage
 * 创建时间：2017/6/29 16:41:09
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;

namespace GFF.WS.Model
{
    [ProtoContract]
    public class WSMessage
    {
        [ProtoMember(1, IsRequired = true, Name = "id")]
        public string ID
        {
            get;
            set;
        }
        [ProtoMember(2, IsRequired = true, Name = "content")]
        public string Content
        {
            get; set;
        }
        [ProtoMember(3, IsRequired = true, Name = "sender")]
        public string Sender
        {
            get;
            set;
        }
        [ProtoMember(4, IsRequired = true, Name = "time")]
        public string time
        {
            get; set;
        }
    }
}
