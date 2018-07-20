using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComImport]
    [Guid("0000000c-0000-0000-C000-000000000046")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IStream : ISequentialStream
    {
        int Seek(
            /* [in] */ ulong dlibMove,
            /* [in] */ uint dwOrigin,
            /* [out] */ out ulong plibNewPosition);

        int SetSize(
            /* [in] */ ulong libNewSize);

        int CopyTo(
            /* [unique][in] */ [In] IStream pstm,
            /* [in] */ ulong cb,
            /* [out] */ out ulong pcbRead,
            /* [out] */ out ulong pcbWritten);

        int Commit(
            /* [in] */ uint grfCommitFlags);

        int Revert();

        int LockRegion(
            /* [in] */ ulong libOffset,
            /* [in] */ ulong cb,
            /* [in] */ uint dwLockType);

        int UnlockRegion(
            /* [in] */ ulong libOffset,
            /* [in] */ ulong cb,
            /* [in] */ uint dwLockType);

        int Stat(
            /* [out] */ out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg,
            /* [in] */ uint grfStatFlag);

        int Clone(
            /* [out] */ out IStream ppstm);

    };
}
