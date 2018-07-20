using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown), 
    Guid("0c733a30-2a1c-11ce-ade5-00aa0044773d")]
    public interface ISequentialStream
    {
        int Read(
            /* [length_is][size_is][out] */ IntPtr pv,
            /* [in] */ uint cb,
            /* [out] */ out uint pcbRead);

        int Write(
            /* [size_is][in] */ IntPtr pv,
            /* [in] */ uint cb,
            /* [out] */ out uint pcbWritten);

    };
}
