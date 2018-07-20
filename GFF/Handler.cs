/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：7c6eef21-01f8-411f-819e-f12113296bb2
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF
 * 类名称：Handler
 * 创建时间：2017/6/20 16:30:52
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GFF
{
    #region client

    public delegate void OnLoginedHandler(object sender, string msg);

    public delegate void OnSubedHandler(string msg);

    #endregion
}
