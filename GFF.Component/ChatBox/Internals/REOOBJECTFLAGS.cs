using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [Flags, ComVisible(false)]
    public enum REOOBJECTFLAGS : uint
    {
        REO_NULL = 0x00000000,	// No flags
        REO_READWRITEMASK = 0x0000003F,	// Mask out RO bits
        REO_DONTNEEDPALETTE = 0x00000020,	// Object doesn't need palette
        REO_BLANK = 0x00000010,	// Object is blank
        REO_DYNAMICSIZE = 0x00000008,	// Object defines size always
        REO_INVERTEDSELECT = 0x00000004,	// Object drawn all inverted if sel
        REO_BELOWBASELINE = 0x00000002,	// Object sits below the baseline
        REO_RESIZABLE = 0x00000001,	// Object may be resized
        REO_LINK = 0x80000000,	// Object is a link (RO)
        REO_STATIC = 0x40000000,	// Object is static (RO)
        REO_SELECTED = 0x08000000,	// Object selected (RO)
        REO_OPEN = 0x04000000,	// Object open in its server (RO)
        REO_INPLACEACTIVE = 0x02000000,	// Object in place active (RO)
        REO_HILITED = 0x01000000,	// Object is to be hilited (RO)
        REO_LINKAVAILABLE = 0x00800000,	// Link believed available (RO)
        REO_GETMETAFILE = 0x00400000	// Object requires metafile (RO)
    }
}
