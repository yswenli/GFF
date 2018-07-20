/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：2f332a1e-ef68-4722-8500-52d7c6493a53
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper.Extention
 * 类名称：ExtentionHelper
 * 创建时间：2016/11/11 16:11:25
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace GFF.Helper.Extention
{
    public static class ExtentionHelper
    {
        /// <summary>
        /// winform控件UI多线程处理
        /// </summary>
        /// <param name="control"></param>
        /// <param name="action"></param>
        public static void InvokeAction(this Control control, Action action)
        {
            if (control.IsHandleCreated)
            {
                if (control.InvokeRequired)
                {
                    control.BeginInvoke(action);
                }
                else
                {
                    action();
                }
            }
        }

        public static List<string> ToList(this string[] arr)
        {
            List<string> list = new List<string>();
            if (arr != null && arr.Length > 0)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    list.Add(arr[i]);
                }
            }
            return list;
        }
    }
}
