using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(true),
     ComImport(),
     Guid("00000103-0000-0000-C000-000000000046"),
     InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumFORMATETC
    {
        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Next(
            [In, MarshalAs(UnmanagedType.U4)]
			int celt,
            [Out]
			FORMATETC rgelt,
            [In, Out, MarshalAs(UnmanagedType.LPArray)]
			int[] pceltFetched);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Skip(
            [In, MarshalAs(UnmanagedType.U4)]
			int celt);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Reset();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Clone(
            [Out, MarshalAs(UnmanagedType.LPArray)]
			IEnumFORMATETC[] ppenum);
    }
}
