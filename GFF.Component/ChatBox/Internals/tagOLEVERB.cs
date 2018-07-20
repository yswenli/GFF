using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(false), StructLayout(LayoutKind.Sequential)]
    public sealed class tagOLEVERB
    {
        [MarshalAs(UnmanagedType.I4)]
        public int lVerb;

        [MarshalAs(UnmanagedType.LPWStr)]
        public String lpszVerbName;

        [MarshalAs(UnmanagedType.U4)]
        public int fuFlags;

        [MarshalAs(UnmanagedType.U4)]
        public int grfAttribs;

    }
}
