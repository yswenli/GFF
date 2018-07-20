using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    internal class NativeMethods
    {
        public const int WM_USER = 0x0400;
        public const int EM_GETOLEINTERFACE = WM_USER + 60;

        [DllImport("User32.dll", 
            CharSet = CharSet.Auto, 
            PreserveSig = false)]
        public static extern IRichEditOle SendMessage(
            IntPtr hWnd, int message, int wParam);

        [DllImport("ole32.dll")]
        public static extern int CreateILockBytesOnHGlobal(
            IntPtr hGlobal, bool fDeleteOnRelease, out ILockBytes ppLkbyt);

        [DllImport("ole32.dll")]
        public static extern int StgCreateDocfileOnILockBytes(
            ILockBytes plkbyt, uint grfMode, uint reserved, out IStorage ppstgOpen);

        [DllImport("ole32.dll")]
        public static extern int OleCreateFromFile([In] ref Guid rclsid,
            [MarshalAs(UnmanagedType.LPWStr)] string lpszFileName, 
            [In] ref Guid riid,
            uint renderopt, 
            ref FORMATETC pFormatEtc, 
            IOleClientSite pClientSite,
            IStorage pStg, 
            [MarshalAs(UnmanagedType.IUnknown)] out object ppvObj);

        [DllImport("ole32.dll")]
        public static extern int OleSetContainedObject(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk, bool fContained);
    }
}
