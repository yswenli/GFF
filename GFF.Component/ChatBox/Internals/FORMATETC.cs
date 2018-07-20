using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [StructLayout(LayoutKind.Sequential), ComVisible(false)]
    public struct FORMATETC
    {
        public CLIPFORMAT cfFormat;
        public IntPtr ptd;
        public DVASPECT dwAspect;
        public int lindex;
        public TYMED tymed;
    }
}
