/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：7217fe90-0011-4cc2-833f-db98fafa37b0
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper.Win32
 * 类名称：HookHelper
 * 创建时间：2016/12/27 14:05:26
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace GFF.Helper.Win32
{
    /// <summary>
    /// 全局钩子
    /// </summary>
    public class HookHelper
    {
        #region Win32
        [DllImport("user32.dll")]   //设置钩子  第二个参数为回调函数指针
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hmod, int dwThreadid);
        [DllImport("user32.dll")]   //传递到下一个钩子
        public static extern int CallNextHookEx(IntPtr hHook, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]   //卸载钩子
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);
        [DllImport("kernel32.dll")] //获取模块句柄  
        public static extern IntPtr GetModuleHandle(string lpModuleName);
        #endregion

        #region kyeborad
        private const int WH_KEYBOARD_LL = 13;  //钩子类型 全局钩子
        private const int WM_KEYUP = 0x101;     //按键抬起
        private const int WM_KEYDOWN = 0x100;   //按键按下
        public delegate int HookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void KeyHookHanlder(object sender, KeyHookEventArgs e);
        public event KeyHookHanlder KeyHookEvent;
        //
        public struct KeyInfoStruct
        {
            public int vkCode;        //按键键码
            public int scanCode;
            public int flags;       //键盘是否按下的标志
            public int time;
            public int dwExtraInfo;
        }
        private IntPtr hHook;
        private GCHandle gc;
        protected virtual void OnKeyHookEvent(KeyHookEventArgs e)
        {
            if (KeyHookEvent != null)
                this.KeyHookEvent(this, e);
        }
        private int KeyHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && KeyHookEvent != null)
            {
                KeyInfoStruct inputInfo = (KeyInfoStruct)Marshal.PtrToStructure(lParam, typeof(KeyInfoStruct));
                if (wParam == (IntPtr)WM_KEYDOWN)
                {//如果按键按下
                    this.OnKeyHookEvent(new KeyHookEventArgs(inputInfo.vkCode));
                }
            }
            return CallNextHookEx(hHook, nCode, wParam, lParam);//继续传递消息
        }
        /// <summary>
        /// 注册键盘Hook
        /// </summary>
        /// <returns></returns>
        public IntPtr SetHook()
        {
            if (hHook == IntPtr.Zero)
            {
                HookProc keyCallBack = new HookProc(KeyHookProcedure);
                hHook = SetWindowsHookEx(
                    WH_KEYBOARD_LL, keyCallBack,
                    GetModuleHandle(System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName),
                    0);
                gc = GCHandle.Alloc(keyCallBack);
            }
            return hHook;
        }
        /// <summary>
        /// 卸载键盘Hook
        /// </summary>
        /// <returns></returns>
        public IntPtr UnLoadHook()
        {
            if (hHook != IntPtr.Zero)
            {
                if (UnhookWindowsHookEx(hHook))
                    hHook = IntPtr.Zero;
            }
            return hHook;
        }
        #endregion

        #region mouse
        private const int WH_MOUSE_LL = 14;//全局鼠标Hook 7是局部的 13全局键盘 2局部键盘
        private const uint WM_LBUTTONDOWN = 0x201;
        private const uint WM_LBUTTONUP = 0x202;
        private const uint WM_RBUTTONDOWN = 0x204;
        private const uint WM_RBUTTONUP = 0x205;
        public delegate int MHookProc(int nCode, IntPtr wParam, IntPtr lParam);
        public delegate void MHookEventHandler(object sender, MHookEventArgs e);
        public event MHookEventHandler MHookEvent;
        //
        public struct POINT
        {
            public int X;
            public int Y;
        }
        /// <summary>
        /// 鼠标结构信息
        /// </summary>
        public struct MSLLHOOTSTRUCT
        {
            public POINT pt;
            public int mouseData;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        private IntPtr mHook;
        public IntPtr HHook
        {
            get
            {
                return mHook;
            }
        }
        GCHandle mgc;
        /// <summary>
        /// 鼠标Hook回调函数
        /// </summary>
        /// <param name="nCode"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        private int MouseHookProcedure(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && MHookEvent != null)
            {
                MSLLHOOTSTRUCT stMSLL = (MSLLHOOTSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOTSTRUCT));
                ButtonStatus btnStatus = ButtonStatus.None;
                if (wParam == (IntPtr)WM_LBUTTONDOWN)
                    btnStatus = ButtonStatus.LeftDown;
                else if (wParam == (IntPtr)WM_LBUTTONUP)
                    btnStatus = ButtonStatus.LeftUp;
                else if (wParam == (IntPtr)WM_RBUTTONDOWN)
                    btnStatus = ButtonStatus.RightDown;
                else if (wParam == (IntPtr)WM_RBUTTONUP)
                    btnStatus = ButtonStatus.RightUp;
                MHookEvent(this, new MHookEventArgs(btnStatus, stMSLL.pt.X, stMSLL.pt.Y));
            }
            return CallNextHookEx(mHook, nCode, wParam, lParam);
        }
        /// <summary>
        /// 设置鼠标Hook
        /// </summary>
        /// <returns></returns>
        public bool SetMHook()
        {
            if (mHook != IntPtr.Zero)
                return false;
            HookProc mouseCallBack = new HookProc(MouseHookProcedure);
            mHook = SetWindowsHookEx(WH_MOUSE_LL, mouseCallBack,
                GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (mHook != IntPtr.Zero)
            {
                mgc = GCHandle.Alloc(mouseCallBack);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 卸载鼠标Hook
        /// </summary>
        /// <returns></returns>
        public bool UnLoadMHook()
        {
            if (mHook == IntPtr.Zero)
                return false;
            if (UnhookWindowsHookEx(mHook))
            {
                mHook = IntPtr.Zero;
                mgc.Free();
                return true;
            }
            return false;
        }
        #endregion
    }



    public class KeyHookEventArgs : EventArgs
    {
        private int keyCode;
        public int KeyCode
        {
            get
            {
                return keyCode;
            }
        }

        public KeyHookEventArgs(int code)
        {
            this.keyCode = code;
        }

    }
    //鼠标状态枚举值
    public enum ButtonStatus
    {
        LeftDown,
        LeftUp,
        RightDown,
        RightUp,
        None
    }
    //鼠标事件参数
    public class MHookEventArgs : EventArgs
    {
        private ButtonStatus mButton;
        public ButtonStatus MButton
        {
            get
            {
                return mButton;
            }
        }

        private int x;
        public int X
        {
            get
            {
                return x;
            }
        }

        private int y;
        public int Y
        {
            get
            {
                return y;
            }
        }

        public MHookEventArgs(ButtonStatus btn, int cx, int cy)
        {
            this.mButton = btn;
            this.x = cx;
            this.y = cy;
        }
    }
}
