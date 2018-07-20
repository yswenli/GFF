/*****************************************************************************************************
 * 本代码版权归@wenli所有，All Rights Reserved (C) 2015-2017
 *****************************************************************************************************
 * CLR版本：4.0.30319.42000
 * 唯一标识：8ebf757b-628b-495f-bf08-07e92a3e0da8
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 项目名称：$projectname$
 * 命名空间：GFF.Helper.Win32
 * 类名称：WindowsHelper
 * 创建时间：2017/2/20 17:09:56
 * 创建人：wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace GFF.Helper.Win32
{
    /// <summary>
    /// windows相关操作
    /// </summary>
    public static class WindowsHelper
    {
        #region ShowQuery
        public static bool ShowQuery(string query)
        {
            if (DialogResult.Yes != MessageBox.Show(query, "提示", MessageBoxButtons.YesNo))
            {
                return false;
            }

            return true;
        }
        #endregion

        #region DoWindowsEvents
        /// <summary>
        /// DoWindowsEvents 在UI线程中调用该方法将使UI线程处理windows事件。
        /// </summary>
        public static void DoWindowsEvents()
        {
            Application.DoEvents();
        }
        #endregion

        #region GetMdiChildForm ,MdiChildIsExist
        public static Form GetMdiChildForm(Form parentForm, Type childFormType)
        {
            foreach (Form child in parentForm.MdiChildren)
            {
                if (child.GetType() == childFormType)
                {
                    return child;
                }
            }

            return null;
        }

        public static bool MdiChildIsExist(Form parentForm, Type childFormType)
        {
            if (WindowsHelper.GetMdiChildForm(parentForm, childFormType) != null)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region GetCursorPosition
        public static Point GetCursorPosition()
        {
            return System.Windows.Forms.Cursor.Position;
        }
        #endregion

        #region GetStartupDirectoryPath
        /// <summary>
        /// GetStartupDirectoryPath 获取当前应用程序所在的目录
        /// </summary>        
        public static string GetStartupDirectoryPath()
        {
            return Application.StartupPath; //AppDomain.CurrentDomain.BaseDirectory
        }
        #endregion

        #region MouseEvent 模拟鼠标/键盘点击
        /// <summary>
        /// MouseEvent 模拟鼠标点击
        /// </summary>       
        [DllImport("user32.dll", EntryPoint = "mouse_event")]
        public static extern void MouseEvent(MouseEventFlag flags, int dx, int dy, uint data, UIntPtr extraInfo);

        //在(34, 258)处点击鼠标
        //SetCursorPos(34, 258);
        //MouseEvent(MouseEventFlag.LeftDown, 0, 0, 0, UIntPtr.Zero);
        //MouseEvent(MouseEventFlag.LeftUp, 0, 0, 0, UIntPtr.Zero);

        /// <summary>
        /// SetCursorPos 设置光标的绝对位置
        /// </summary>  
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// KeybdEvent 模拟键盘。已经过期。
        /// </summary>
        [DllImport("user32.dll", EntryPoint = "keybd_event")]
        public static extern void KeybdEvent2(byte key, byte bScan, KeybdEventFlag flags, uint dwExtraInfo);

        public static void KeybdEvent(byte key, byte bScan, KeybdEventFlag flags, uint dwExtraInfo)
        {
            INPUT input = new INPUT();
            input.type = 1; //keyboard
            input.ki.wVk = key;
            input.ki.wScan = MapVirtualKey(key, 0);
            input.ki.dwFlags = (int)flags;
            SendInput(1, ref input, Marshal.SizeOf(input));
        }

        [DllImport("user32.dll")]
        private static extern UInt32 SendInput(UInt32 nInputs, ref INPUT pInputs, int cbSize);
        [DllImport("user32.dll")]
        private static extern byte MapVirtualKey(byte wCode, int wMap);
        #endregion

        #region 根据扩展名获取系统图标
        /// <summary>
        /// 根据扩展名获取系统图标。
        /// </summary> 
        /// <param name="fileType">文件类型，使用扩展名表示，如".txt"</param>      
        public static Icon GetSystemIconByFileType(string fileType, bool isLarge)
        {
            if (isLarge)
            {
                return GetIcon(fileType, FILE_ATTRIBUTE.NORMAL, SHGFI.USEFILEATTRIBUTES | SHGFI.ICON | SHGFI.LARGEICON);
            }

            return GetIcon(fileType, FILE_ATTRIBUTE.NORMAL, SHGFI.USEFILEATTRIBUTES | SHGFI.ICON | SHGFI.SMALLICON);
        }

        private static Icon GetIcon(string path, FILE_ATTRIBUTE dwAttr, SHGFI dwFlag)
        {
            SHFILEINFO fi = new SHFILEINFO();
            Icon ic = null;
            int iTotal = (int)SHGetFileInfo(path, dwAttr, ref fi, 0, dwFlag);
            ic = Icon.FromHandle(fi.hIcon);
            return ic;
        }

        #region PInvoke
        [DllImport("shell32", EntryPoint = "SHGetFileInfo", ExactSpelling = false, CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SHGetFileInfo(string pszPath, FILE_ATTRIBUTE dwFileAttributes, ref SHFILEINFO sfi, int cbFileInfo, SHGFI uFlags);


        // Contains information about a file object
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct SHFILEINFO
        {
            public IntPtr hIcon;    //文件的图标句柄
            public IntPtr iIcon;    //图标的系统索引号
            public uint dwAttributes;   //文件的属性值
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;  //文件的显示名
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;   //文件的类型名
        };

        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        public enum SHGFI : uint
        {
            ADDOVERLAYS = 0x20,
            ATTR_SPECIFIED = 0x20000,
            ATTRIBUTES = 0x800,   //获得属性
            DISPLAYNAME = 0x200,  //获得显示名
            EXETYPE = 0x2000,
            ICON = 0x100,    //获得图标
            ICONLOCATION = 0x1000,
            LARGEICON = 0,    //获得大图标
            LINKOVERLAY = 0x8000,
            OPENICON = 2,
            OVERLAYINDEX = 0x40,
            PIDL = 8,
            SELECTED = 0x10000,
            SHELLICONSIZE = 4,
            SMALLICON = 1,    //获得小图标
            SYSICONINDEX = 0x4000,
            TYPENAME = 0x400,   //获得类型名
            USEFILEATTRIBUTES = 0x10
        }
        // Flags that specify the file information to retrieve with SHGetFileInfo
        [Flags]
        public enum FILE_ATTRIBUTE
        {
            READONLY = 0x00000001,
            HIDDEN = 0x00000002,
            SYSTEM = 0x00000004,
            DIRECTORY = 0x00000010,
            ARCHIVE = 0x00000020,
            DEVICE = 0x00000040,
            NORMAL = 0x00000080,
            TEMPORARY = 0x00000100,
            SPARSE_FILE = 0x00000200,
            REPARSE_POINT = 0x00000400,
            COMPRESSED = 0x00000800,
            OFFLINE = 0x00001000,
            NOT_CONTENT_INDEXED = 0x00002000,
            ENCRYPTED = 0x00004000
        }
        #endregion
        #endregion

        #region 开机自动启动
        /// <summary> 
        /// 开机自动启动,使用注册表 
        /// </summary> 
        /// <param name=\"Started\">是否开机自动启动</param> 
        /// <param name=\"name\">取一个唯一的注册表Key名称</param> 
        /// <param name=\"path\">启动程序的完整路径</param> 
        public static void RunWhenStart_usingReg(bool started, string name, string path)
        {
            RegistryKey HKLM = Registry.LocalMachine;
            try
            {
                RegistryKey run = HKLM.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run\\");
                if (started)
                {
                    run.SetValue(name, path);
                }
                else
                {
                    object val = run.GetValue(name);
                    if (val != null)
                    {
                        run.DeleteValue(name);
                    }
                }
            }
            finally
            {
                HKLM.Close();
            }
        }
        #endregion      

        #region CaptureScreenImage
        public static Image CaptureScreenImage()
        {
            //可以抓悬浮窗等LayeredWindow    
            Size sz = Screen.PrimaryScreen.Bounds.Size;
            IntPtr hDesk = GetDesktopWindow();
            IntPtr hSrce = GetWindowDC(hDesk);
            IntPtr hDest = CreateCompatibleDC(hSrce);
            IntPtr hBmp = CreateCompatibleBitmap(hSrce, sz.Width, sz.Height);
            IntPtr hOldBmp = SelectObject(hDest, hBmp);
            bool b = BitBlt(hDest, 0, 0, sz.Width, sz.Height, hSrce, 0, 0, CopyPixelOperation.SourceCopy | CopyPixelOperation.CaptureBlt);
            Bitmap bmp = Bitmap.FromHbitmap(hBmp);
            SelectObject(hDest, hOldBmp);
            DeleteObject(hBmp);
            DeleteDC(hDest);
            ReleaseDC(hDesk, hSrce);
            return bmp;
        }

        [DllImport("gdi32.dll")]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        private static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr ptr);
        #endregion

        #region SetForegroundWindow
        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        private static extern bool SetActiveWindow(IntPtr hWnd);
        /// <summary>
        /// 设置目标窗体为活动窗体。将其TopLevel在最前。
        /// </summary>     
        public static void SetForegroundWindow(Form window)
        {
            SetActiveWindow(window.Handle);
        }
        #endregion
    }

    #region MouseEventFlag ,KeybdEventFlag
    /// <summary>
    /// MouseEventFlag 模拟鼠标点击的相关标志。
    /// </summary>
    [Flags]
    public enum MouseEventFlag : uint
    {
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesk = 0x4000,
        Absolute = 0x8000 //绝对坐标
    }

    /// <summary>
    /// KeybdEventFlag 模拟键盘点击的相关标志。
    /// </summary>
    [Flags]
    public enum KeybdEventFlag : uint
    {
        Down = 0,
        Up = 0x0002
    }
    #endregion

    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public Int32 type;
        [FieldOffset(4)]
        public KEYBDINPUT ki;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
        [FieldOffset(4)]
        public HARDWAREINPUT hi;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public Int32 dx;
        public Int32 dy;
        public Int32 mouseData;
        public Int32 dwFlags;
        public Int32 time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct KEYBDINPUT
    {
        public Int16 wVk;
        public Int16 wScan;
        public Int32 dwFlags;
        public Int32 time;
        public IntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    public struct HARDWAREINPUT
    {
        public Int32 uMsg;
        public Int16 wParamL;
        public Int16 wParamH;
    }

}
