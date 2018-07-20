using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{    
    [StructLayout(LayoutKind.Sequential)]
    public struct CHARRANGE
    {
        public int cpMin;
        public int cpMax;
    }
}
