/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：115d8aaa-502d-4c1d-a0d7-d3dedd4ea90a
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper.Win32
 * 类名称：SendMessageClass
 * 创建时间：2016/11/9 14:42:49
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GFF.Helper.Win32
{

    #region SendMessageClass

    public class SendMessageClass
    {
        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            int lParam
        );

        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            IntPtr lParam
        );

        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            StringBuilder lParam
        );

        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            int wParam,
            string lParam
        );

        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            bool wParam,
            string lParam
        );

        [DllImport("USER32.DLL")]
        public static extern int SendMessage(
            IntPtr hwnd,
            int wMsg,
            bool wParam,
            int lParam
        );
    }

    #endregion
}