using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;

namespace GFF.Component.ChatBox.Internals
{
    [ComVisible(true), 
    ComImport(), 
    Guid("00000112-0000-0000-C000-000000000046"), 
    InterfaceTypeAttribute(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IOleObject
    {

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetClientSite(
            [In, MarshalAs(UnmanagedType.Interface)]
			IOleClientSite pClientSite);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetClientSite(out IOleClientSite site);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetHostNames(
            [In, MarshalAs(UnmanagedType.LPWStr)]
			string szContainerApp,
            [In, MarshalAs(UnmanagedType.LPWStr)]
			string szContainerObj);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Close(
            [In, MarshalAs(UnmanagedType.I4)]
			int dwSaveOption);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetMoniker(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwWhichMoniker,
            [In, MarshalAs(UnmanagedType.Interface)]
			object pmk);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMoniker(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwAssign,
            [In, MarshalAs(UnmanagedType.U4)]
			int dwWhichMoniker,
            out object moniker);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int InitFromData(
            [In, MarshalAs(UnmanagedType.Interface)]
			IDataObject pDataObject,
            [In, MarshalAs(UnmanagedType.I4)]
			int fCreation,
            [In, MarshalAs(UnmanagedType.U4)]
			int dwReserved);

        int GetClipboardData(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwReserved,
            out IDataObject data);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int DoVerb(
            [In, MarshalAs(UnmanagedType.I4)]
			int iVerb,
            [In]
			IntPtr lpmsg,
            [In, MarshalAs(UnmanagedType.Interface)]
			IOleClientSite pActiveSite,
            [In, MarshalAs(UnmanagedType.I4)]
			int lindex,
            [In]
			IntPtr hwndParent,
            [In]
			COMRECT lprcPosRect);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumVerbs(out IEnumOLEVERB e);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Update();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int IsUpToDate();

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserClassID(
            [In, Out]
			ref Guid pClsid);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetUserType(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwFormOfType,
            [Out, MarshalAs(UnmanagedType.LPWStr)]
			out string userType);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetExtent(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwDrawAspect,
            [In]
			Size pSizel);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetExtent(
            [In, MarshalAs(UnmanagedType.U4)]
			int dwDrawAspect,
            [Out]
			Size pSizel);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Advise([In, MarshalAs(UnmanagedType.Interface)] IAdviseSink pAdvSink, out int cookie);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int Unadvise([In, MarshalAs(UnmanagedType.U4)] int dwConnection);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int EnumAdvise(out IEnumSTATDATA e);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int GetMiscStatus([In, MarshalAs(UnmanagedType.U4)] int dwAspect, out int misc);

        [return: MarshalAs(UnmanagedType.I4)]
        [PreserveSig]
        int SetColorScheme([In] tagLOGPALETTE pLogpal);
    }
}
