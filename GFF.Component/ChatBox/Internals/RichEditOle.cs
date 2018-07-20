using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;

namespace GFF.Component.ChatBox.Internals
{
    internal class RichEditOle
    {
        private ChatBox agileRichTextBox;
        private IRichEditOle richEditOle;

        #region Ctor
        public RichEditOle(ChatBox _richEdit)
        {
            agileRichTextBox = _richEdit;
        }

        public IRichEditOle IRichEditOle
        {
            get
            {
                if (richEditOle == null)
                {
                    richEditOle = NativeMethods.SendMessage(
                        agileRichTextBox.Handle, NativeMethods.EM_GETOLEINTERFACE, 0);
                }
                return richEditOle;
            }
        } 
        #endregion

        #region InsertControl
        public void InsertControl(Control control)
        {
            this.InsertControl(control, this.agileRichTextBox.TextLength ,1);
        }

        public void InsertControl(Control control, int position, uint dwUser)
        {
            if (control == null)
            {
                return;
            }
            ILockBytes bytes;
            IStorage storage;
            IOleClientSite site;
            Guid guid = Marshal.GenerateGuidForType(control.GetType());
            NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out bytes);
            NativeMethods.StgCreateDocfileOnILockBytes(bytes, 0x1012, 0, out storage);
            IRichEditOle.GetClientSite(out site);
            REOBJECT lpreobject = new REOBJECT();
            lpreobject.posistion = position;
            lpreobject.clsid = guid;
            lpreobject.pstg = storage;
            lpreobject.poleobj = Marshal.GetIUnknownForObject(control);
            lpreobject.polesite = site;
            lpreobject.dvAspect = 1;
            lpreobject.dwFlags = 2;
            lpreobject.dwUser = dwUser;
            IRichEditOle.InsertObject(lpreobject);
            Marshal.ReleaseComObject(bytes);
            Marshal.ReleaseComObject(site);
            Marshal.ReleaseComObject(storage);
        } 
        #endregion

        #region InsertImageFromFile
        public bool InsertImageFromFile(string strFilename)
        {
            return this.InsertImageFromFile(strFilename, this.agileRichTextBox.TextLength);
        }

        public bool InsertImageFromFile(string strFilename, int position)
        {
            ILockBytes bytes;
            IStorage storage;
            IOleClientSite site;
            object obj2;
            NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out bytes);
            NativeMethods.StgCreateDocfileOnILockBytes(bytes, 0x1012, 0, out storage);
            IRichEditOle.GetClientSite(out site);
            FORMATETC pFormatEtc = new FORMATETC();
            pFormatEtc.cfFormat = (CLIPFORMAT)0;
            pFormatEtc.ptd = IntPtr.Zero;
            pFormatEtc.dwAspect = DVASPECT.DVASPECT_CONTENT;
            pFormatEtc.lindex = -1;
            pFormatEtc.tymed = TYMED.TYMED_NULL;
            Guid riid = new Guid("{00000112-0000-0000-C000-000000000046}");
            Guid rclsid = new Guid("{00000000-0000-0000-0000-000000000000}");
            NativeMethods.OleCreateFromFile(ref rclsid, strFilename, ref riid, 1, ref pFormatEtc, site, storage, out obj2);
            if (obj2 == null)
            {
                Marshal.ReleaseComObject(bytes);
                Marshal.ReleaseComObject(site);
                Marshal.ReleaseComObject(storage);
                return false;
            }
            IOleObject pUnk = (IOleObject)obj2;
            Guid pClsid = new Guid();
            pUnk.GetUserClassID(ref pClsid);
            NativeMethods.OleSetContainedObject(pUnk, true);
            REOBJECT lpreobject = new REOBJECT();
            lpreobject.posistion = position;
            lpreobject.clsid = pClsid;
            lpreobject.pstg = storage;
            lpreobject.poleobj = Marshal.GetIUnknownForObject(pUnk);
            lpreobject.polesite = site;
            lpreobject.dvAspect = 1;
            lpreobject.dwFlags = 2;
            lpreobject.dwUser = 0;
            IRichEditOle.InsertObject(lpreobject);
            Marshal.ReleaseComObject(bytes);
            Marshal.ReleaseComObject(site);
            Marshal.ReleaseComObject(storage);
            Marshal.ReleaseComObject(pUnk);
            return true;
        } 
        #endregion

        #region InsertOleObject
        public REOBJECT InsertOleObject(IOleObject oleObject, int index)
        {
            return this.InsertOleObject(oleObject, index, agileRichTextBox.TextLength);
        }

        public REOBJECT InsertOleObject(IOleObject oleObject, int index, int pos)
        {
            if (oleObject == null)
            {
                return null;
            }

            ILockBytes pLockBytes;
            NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out pLockBytes);

            IStorage pStorage;
            NativeMethods.StgCreateDocfileOnILockBytes(
                pLockBytes,
                (uint)(STGM.STGM_SHARE_EXCLUSIVE | STGM.STGM_CREATE | STGM.STGM_READWRITE),
                0,
                out pStorage);

            IOleClientSite pOleClientSite;
            IRichEditOle.GetClientSite(out pOleClientSite);

            Guid guid = new Guid();

            oleObject.GetUserClassID(ref guid);
            NativeMethods.OleSetContainedObject(oleObject, true);

            REOBJECT reoObject = new REOBJECT();

            reoObject.posistion = pos;
            reoObject.clsid = guid;
            reoObject.pstg = pStorage;
            reoObject.poleobj = Marshal.GetIUnknownForObject(oleObject);
            reoObject.polesite = pOleClientSite;
            reoObject.dvAspect = (uint)DVASPECT.DVASPECT_CONTENT;
            reoObject.dwFlags = (uint)REOOBJECTFLAGS.REO_BELOWBASELINE;
            reoObject.dwUser = (uint)index;

            IRichEditOle.InsertObject(reoObject);

            Marshal.ReleaseComObject(pLockBytes);
            Marshal.ReleaseComObject(pOleClientSite);
            Marshal.ReleaseComObject(pStorage);

            return reoObject;
        } 
        #endregion

        public void UpdateObjects()
        {
            int objectCount = this.IRichEditOle.GetObjectCount();
            for (int i = 0; i < objectCount; i++)
            {
                REOBJECT lpreobject = new REOBJECT();
                IRichEditOle.GetObject(i, lpreobject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);
                Point positionFromCharIndex = this.agileRichTextBox.GetPositionFromCharIndex(lpreobject.posistion);
                Rectangle rc = new Rectangle(positionFromCharIndex.X, positionFromCharIndex.Y, 50, 50);
                agileRichTextBox.Invalidate(rc, false);
            }
        }

        public List<REOBJECT> GetAllREOBJECT()
        {
            List<REOBJECT> list = new List<REOBJECT>();
            int objectCount = this.IRichEditOle.GetObjectCount();
            for (int i = 0; i < objectCount; i++)
            {
                REOBJECT lpreobject = new REOBJECT();
                IRichEditOle.GetObject(i, lpreobject, GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);
                list.Add(lpreobject);
            }
            return list;
        }

        public void UpdateObjects(int pos)
        {
            REOBJECT lpreobject = new REOBJECT();
            IRichEditOle.GetObject(
                pos,
                lpreobject,
                GETOBJECTOPTIONS.REO_GETOBJ_ALL_INTERFACES);
            UpdateObjects(lpreobject);
        }

        public void UpdateObjects(REOBJECT reObj)
        {
            Point positionFromCharIndex = agileRichTextBox.GetPositionFromCharIndex(reObj.posistion);
            Size size = GetSizeFromMillimeter(reObj);
            Rectangle rc = new Rectangle(positionFromCharIndex, size);
            agileRichTextBox.Invalidate(rc, false);
        }

        private Size GetSizeFromMillimeter(REOBJECT lpreobject)
        {
            using (Graphics graphics = Graphics.FromHwnd(agileRichTextBox.Handle))
            {
                Point[] pts = new Point[1];
                graphics.PageUnit = GraphicsUnit.Millimeter;

                pts[0] = new Point(
                    lpreobject.sizel.Width / 100,
                    lpreobject.sizel.Height / 100);
                graphics.TransformPoints(
                    CoordinateSpace.Device,
                    CoordinateSpace.Page,
                    pts);
                return new Size(pts[0]);
            }
        }
    }
}
