/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：a036530f-eaa6-4f29-8329-dbad269ec353
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Core.Tcp
 * 类名称：Mapping
 * 创建时间：2016/12/5 10:21:10
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GFF.Core.Interface;

namespace GFF.Core.Tcp.Model
{
    public class Mapping
    {
        public string ChannelID
        {
            get; set;
        }

        public string UID
        {
            get; set;
        }

        public IUserToken UserToken
        {
            get; set;
        }
    }
}
