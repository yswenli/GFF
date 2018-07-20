using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(false), StructLayout(LayoutKind.Sequential)]
    public sealed class tagLOGPALETTE
    {
        [MarshalAs(UnmanagedType.U2)/*leftover(offset=0, palVersion)*/]
        public short palVersion;

        [MarshalAs(UnmanagedType.U2)/*leftover(offset=2, palNumEntries)*/]
        public short palNumEntries;

        // UNMAPPABLE: palPalEntry: Cannot be used as a structure field.
        //   /** @com.structmap(UNMAPPABLE palPalEntry) */
        //  public UNMAPPABLE palPalEntry;
    }
}
