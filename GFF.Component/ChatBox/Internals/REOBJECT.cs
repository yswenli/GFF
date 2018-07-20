using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GFF.Component.ChatBox.Internals
{
    [StructLayout(LayoutKind.Sequential)]
    public class REOBJECT
    {
        public int cbStruct = Marshal.SizeOf(typeof(REOBJECT));	// Size of structure
        public int posistion;											// Character position of object
        public Guid clsid;										// Class ID of object
        public IntPtr poleobj;								    // OLE object interface
        public IStorage pstg;									// Associated storage interface
        public IOleClientSite polesite;							// Associated client site interface
        public Size sizel;										// Size of object (may be 0,0)
        public uint dvAspect;									// Display aspect to use
        public uint dwFlags;									// Object status flags
        public uint dwUser;										// Dword for user's use

        public override string ToString()
        {
            return string.Format("posistion:{0} ,tag:{1}" ,this.posistion,this.dwUser);
        }
    }
}
