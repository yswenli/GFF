using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(true), 
    Guid("00000105-0000-0000-C000-000000000046"), 
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IEnumSTATDATA
    {

        //C#r: UNDONE (Field in interface) public static readonly    Guid iid;

        void Next(
            [In, MarshalAs(UnmanagedType.U4)]
			int celt,
            [Out]
			STATDATA rgelt,
            [Out, MarshalAs(UnmanagedType.LPArray)]
			int[] pceltFetched);


        void Skip(
            [In, MarshalAs(UnmanagedType.U4)]
			int celt);


        void Reset();


        void Clone(
            [Out, MarshalAs(UnmanagedType.LPArray)]
			IEnumSTATDATA[] ppenum);


    }
}
