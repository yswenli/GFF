using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(true), 
    ComImport(), 
    Guid("00000104-0000-0000-C000-000000000046"), 
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumOLEVERB
    {

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Next(
            [MarshalAs(UnmanagedType.U4)]
			int celt,
            [Out]
			tagOLEVERB rgelt,
            [Out, MarshalAs(UnmanagedType.LPArray)]
			int[] pceltFetched);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Skip(
            [In, MarshalAs(UnmanagedType.U4)]
			int celt);

        void Reset();


        void Clone(
            out IEnumOLEVERB ppenum);


    }
}
