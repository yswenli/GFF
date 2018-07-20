/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：85edf07f-3ef4-4bef-a0e9-847c8107a047
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper.Win32
 * 类名称：MouseAndKeyHelper
 * 创建时间：2016/11/9 14:53:50
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GFF.Helper.Win32
{
    /// <summary>
    /// 鼠标键盘模拟对象
    /// </summary>
    public class MouseAndKeyHelper
    {
        #region dllimport

        public const int RDW_INVALIDATE = 0x1;
        public const int RDW_INTERNALPAINT = 0x2;
        public const int RDW_NOERASE = 0x20;
        public const uint KEYEVENTF_KEYUP = 0x2;//若指定该值，该键将被释放；若未指定该值，该键交被接下
        private const Int32 CURSOR_SHOWING = 0x00000001;
        public const int CWP_SKIPDISABLED = 0x2;   //忽略不可用窗体
        public const int CWP_SKIPINVISIBL = 0x1;   //忽略隐藏的窗体
        public const int CWP_All = 0x0;            //一个都不忽略
        public const uint WM_LBUTTONUP = 0x202;

        //鼠标事件声明
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern void mouse_event(MouseeEventFlag flags, int dx, int dy, uint data, UIntPtr extrainfo);
        [DllImport("user32.dll")]//键盘事件声明;该函数将一虚拟键码翻译（映射）成一扫描码或一字符值，或者将一扫描码翻译成一虚拟键码
        public static extern byte MapVirtualKey(byte wCode, int wMap);
        [DllImport("user32.dll")]//该函数检取指定虚拟键的状态。该状态指定此键是UP状态，DOWN状态，还是被触发的（开关每次按下此键时进行切换）。
        public static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);
        [DllImport("user32.dll")]
        public static extern bool GetCursorInfo(out CURSORINFO pci);
        [DllImport("user32.dll")]//获取桌面的句柄
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]//在桌面找寻子窗体
        public static extern IntPtr ChildWindowFromPointEx(IntPtr pHwnd, LPPOINT pt, uint uFlgs);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]//进行坐标转换 （再窗体内部进行查找）
        public static extern bool ScreenToClient(IntPtr hWnd, out LPPOINT lpPoint);
        [DllImport("user32.dll")]//获得句柄对象的位置
        public static extern bool GetWindowRect(IntPtr hWnd, out LPRECT lpRect);
        [DllImport("user32.dll")]//注册全局热键
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        [DllImport("user32.dll")]//卸载全局热键
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        public static extern bool InvalidateRect(IntPtr hWnd, ref LPRECT lpRect, bool bErase);
        [DllImport("user32.dll", EntryPoint = "GetDCEx", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDCEx(IntPtr hWnd, IntPtr hrgnClip, int flags);
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool RedrawWindow(IntPtr hwnd, ref LPRECT rcUpdate, IntPtr hrgnUpdate, int flags);


        #endregion

        /// <summary>
        /// 指示动作执行在哪个鼠标键上 左中右
        /// </summary>
        public enum ClickOnWhat
        {
            LeftMouse = 0,
            MiddleMouse = 1,
            RightMouse = 2
        }
        /// <summary>
        /// 设置鼠标的位置
        /// </summary>
        public void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }
        /// <summary>
        /// 鼠标动作枚举
        /// </summary>
        public enum MouseeEventFlag : uint
        {
            move = 0x0001,//表明发生移动
            leftdown = 0x0002,//表明接按下鼠标左键
            leftup = 0x0004,//表明松开鼠标左键
            rightdown = 0x0008,//表明按下鼠标右键
            rightup = 0x0010,//表明松开鼠标右键
            middledown = 0x0020,//表明按下鼠标中键
            middleup = 0x0040,//表明松开鼠标中键
            xdown = 0x0080,
            xup = 0x0100,
            wheel = 0x0800,//在Windows NT中如果鼠标有一个轮，表明鼠标轮被移动。移动的数量由dwData给出
            virtualdesk = 0x4000,//
            absolute = 0x8000//表明参数dX，dy含有规范化的绝对坐标
        }
        /// <summary>
        /// 键盘动作枚举
        /// </summary>
        public enum VirtualKeys : byte
        {
            VK_LBUTTON = 1, //鼠标左键 
            VK_RBUTTON = 2,　 //鼠标右键 
            VK_CANCEL = 3,　　　 //Ctrl+Break(通常不需要处理) 
            VK_MBUTTON = 4,　　 //鼠标中键 
            VK_BACK = 8, 　　　 //Backspace 
            VK_TAB = 9,　　　　 //Tab 
            VK_CLEAR = 12,　　　 //Num Lock关闭时的数字键盘5 
            VK_RETURN = 13,　　　//Enter(或者另一个) 
            VK_SHIFT = 16,　　　 //Shift(或者另一个) 
            VK_CONTROL = 17,　　 //Ctrl(或者另一个） 
            VK_MENU = 18,　　　　//Alt(或者另一个) 
            VK_PAUSE = 19,　　　 //Pause 
            VK_CAPITAL = 20,　　 //Caps Lock 
            VK_ESCAPE = 27,　　　//Esc 
            VK_SPACE = 32,　　　 //Spacebar 
            VK_PRIOR = 33,　　　 //Page Up 
            VK_NEXT = 34,　　　　//Page Down 
            VK_END = 35,　　　　 //End 
            VK_HOME = 36,　　　　//Home 
            VK_LEFT = 37,　　　 //左箭头 
            VK_UP = 38,　　　　 //上箭头 
            VK_RIGHT = 39,　　　 //右箭头 
            VK_DOWN = 40,　　　 //下箭头 
            VK_SELECT = 41,　　 //可选 
            VK_PRINT = 42,　　　 //可选 
            VK_EXECUTE = 43,　　 //可选 
            VK_SNAPSHOT = 44,　　//Print Screen 
            VK_INSERT = 45,　　　//Insert 
            VK_DELETE = 46,　　 //Delete 
            VK_HELP = 47,　　 //可选 
            VK_NUM0 = 48, //0
            VK_NUM1 = 49, //1
            VK_NUM2 = 50, //2
            VK_NUM3 = 51, //3
            VK_NUM4 = 52, //4
            VK_NUM5 = 53, //5
            VK_NUM6 = 54, //6
            VK_NUM7 = 55, //7
            VK_NUM8 = 56, //8
            VK_NUM9 = 57, //9
            VK_A = 65, //A
            VK_B = 66, //B
            VK_C = 67, //C
            VK_D = 68, //D
            VK_E = 69, //E
            VK_F = 70, //F
            VK_G = 71, //G
            VK_H = 72, //H
            VK_I = 73, //I
            VK_J = 74, //J
            VK_K = 75, //K
            VK_L = 76, //L
            VK_M = 77, //M
            VK_N = 78, //N
            VK_O = 79, //O
            VK_P = 80, //P
            VK_Q = 81, //Q
            VK_R = 82, //R
            VK_S = 83, //S
            VK_T = 84, //T
            VK_U = 85, //U
            VK_V = 86, //V
            VK_W = 87, //W
            VK_X = 88, //X
            VK_Y = 89, //Y
            VK_Z = 90, //Z
            VK_NUMPAD0 = 96, //0
            VK_NUMPAD1 = 97, //1
            VK_NUMPAD2 = 98, //2
            VK_NUMPAD3 = 99, //3
            VK_NUMPAD4 = 100, //4
            VK_NUMPAD5 = 101, //5
            VK_NUMPAD6 = 102, //6
            VK_NUMPAD7 = 103, //7
            VK_NUMPAD8 = 104, //8
            VK_NUMPAD9 = 105, //9
            VK_NULTIPLY = 106,　 //数字键盘上的* 
            VK_ADD = 107,　　　　//数字键盘上的+ 
            VK_SEPARATOR = 108,　//可选 
            VK_SUBTRACT = 109,　 //数字键盘上的- 
            VK_DECIMAL = 110,　　//数字键盘上的. 
            VK_DIVIDE = 111,　　 //数字键盘上的/
            VK_F1 = 112,
            VK_F2 = 113,
            VK_F3 = 114,
            VK_F4 = 115,
            VK_F5 = 116,
            VK_F6 = 117,
            VK_F7 = 118,
            VK_F8 = 119,
            VK_F9 = 120,
            VK_F10 = 121,
            VK_F11 = 122,
            VK_F12 = 123,
            VK_NUMLOCK = 144,　　//Num Lock 
            VK_SCROLL = 145 　 // Scroll Lock 
        }

        public struct LPPOINT
        {
            public int X;
            public int Y;
        }
        /// <summary>
        /// 光标
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public Point ptScreenPos;
        }
        /// <summary>
        /// 获得句柄对象的位置
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct LPRECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        #region 工具方法
        /// <summary>
        /// 绘制光标
        /// </summary>
        /// <param name="g"></param>
        public static void DrawMouse(Graphics g)
        {
            try
            {
                CURSORINFO pci;
                pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                MouseAndKeyHelper.GetCursorInfo(out pci);
                Cursor cur = new Cursor(pci.hCursor);
                cur.Draw(g, new Rectangle(pci.ptScreenPos.X, pci.ptScreenPos.Y, cur.Size.Width, cur.Size.Height));
            }
            catch { }
        }
        /// <summary>
        /// 获取键盘状态
        /// </summary>
        private bool GetState(VirtualKeys vKey)//Down 返回true Up 返回false
        {
            return (GetKeyState((int)vKey) == 1);//若高序位为1，则键处于DOWN状态，否则为UP状态。
        }
        /// <summary>
        /// 键盘键按下
        /// </summary>
        public void KeyDown(VirtualKeys vKey)
        {
            //if (GetState(vKey) == true)
            keybd_event((byte)vKey, MapVirtualKey((byte)vKey, 0), 0, UIntPtr.Zero);
        }
        /// <summary>
        /// 键盘键抬起
        /// </summary>
        public void KeyUp(VirtualKeys vKey)
        {
            //if (GetState(vKey) == false)
            keybd_event((byte)vKey, MapVirtualKey((byte)vKey, 0), KEYEVENTF_KEYUP, UIntPtr.Zero);
        }
        /// <summary>
        /// 键盘键按下后即抬起
        /// </summary>
        public void KeyPress(VirtualKeys vKey)
        {
            KeyDown(vKey);
            Thread.Sleep(100);
            KeyUp(vKey);
        }
        /// <summary>
        /// 鼠标键的模拟事件
        /// </summary>
        private void MouseEvent(MouseeEventFlag flag, int localX, int localY, uint data)
        {
            mouse_event(flag, localX, localY, data, UIntPtr.Zero);
        }
        /// <summary>
        /// 鼠标键的按下
        /// </summary>
        public void MouseDown(ClickOnWhat clickOnWhat)
        {
            if (clickOnWhat == ClickOnWhat.LeftMouse)
            {
                mouse_event(MouseeEventFlag.leftdown, 0, 0, 0, UIntPtr.Zero);
                ;
            }
            else if (clickOnWhat == ClickOnWhat.MiddleMouse)
            {
                mouse_event(MouseeEventFlag.middledown, 0, 0, 0, UIntPtr.Zero);
            }
            else if (clickOnWhat == ClickOnWhat.RightMouse)
            {
                mouse_event(MouseeEventFlag.rightdown, 0, 0, 0, UIntPtr.Zero);
            }
        }
        /// <summary>
        /// 鼠标键的抬起
        /// </summary>
        public void MouseUp(ClickOnWhat clickOnWhat)
        {
            if (clickOnWhat == ClickOnWhat.LeftMouse)
            {
                mouse_event(MouseeEventFlag.leftup, 0, 0, 0, UIntPtr.Zero);
            }
            else if (clickOnWhat == ClickOnWhat.MiddleMouse)
            {
                mouse_event(MouseeEventFlag.middleup, 0, 0, 0, UIntPtr.Zero);
            }
            else if (clickOnWhat == ClickOnWhat.RightMouse)
            {
                mouse_event(MouseeEventFlag.rightup, 0, 0, 0, UIntPtr.Zero);
            }
        }
        /// <summary>
        /// 鼠标滚轮滚动
        /// </summary>
        public void MouseWheel(int delta)
        {
            mouse_event(MouseeEventFlag.wheel, 0, 0, (uint)delta, UIntPtr.Zero);
        }

        /// <summary>
        /// 是否开机启动
        /// </summary>
        /// <param name="isAuto"></param>
        /// <param name="keyName"></param>
        public static void AutoRun(bool isAuto, string keyName)
        {
            RegistryKey regKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\", true);
            if (isAuto)
            {
                if (regKey == null)
                    regKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run\");
                regKey.SetValue(keyName, Application.ExecutablePath);
            }
            else
            {
                if (regKey != null)
                {
                    if (regKey.GetValue(keyName) != null)
                        regKey.DeleteValue(keyName);
                }
            }
            regKey.Close();
        }


        private const uint WM_HOTKEY = 0x312;
        private const uint MOD_ALT = 0x1;
        private const uint MOD_CONTROL = 0x2;
        private const uint MOD_SHIFT = 0x4;

        //保存当前设置的临时变量
        private static uint comboKeys;

        /// <summary>
        /// 注册热键
        /// </summary>
        /// <param name="control"></param>
        /// <param name="hasCtrl"></param>
        /// <param name="hasAlt"></param>
        /// <param name="hasShift"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool RegistHotKeys(Control control, bool hasCtrl, bool hasAlt, bool hasShift, string key, int hotKeyID)
        {
            bool result = false;

            MouseAndKeyHelper.UnregisterHotKey(control.Handle, hotKeyID);

            comboKeys = 0 | (hasCtrl ? MOD_CONTROL : 0) | (hasAlt ? MOD_ALT : 0) | (hasShift ? MOD_SHIFT : 0);

            uint keyCode = Convert.ToUInt32((Keys)Enum.Parse(typeof(Keys), key));

            if (MouseAndKeyHelper.RegisterHotKey(control.Handle, hotKeyID, comboKeys, keyCode))
            {
                result = true;
            }
            else
            {
                //throw new Exception("注册热键失败！");
            }
            return result;
        }

        #endregion
    }
}
