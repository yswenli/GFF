using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Threading;
using System.Drawing.Imaging;

namespace GFF.Component.Capture
{
    public partial class FrmOut : Form
    {
        public FrmOut(Bitmap bmp)
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            m_bmp = bmp;

            this.FormClosing += (s, e) => m_bmp.Dispose();

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private Bitmap m_bmp;
        public Bitmap Bmp
        {
            get { return m_bmp; }
        }

        private Point m_ptOriginal;
        private bool m_bMouseEnter;
        private bool m_bLoad;
        private bool m_bMinimum;
        private bool m_bMaxmum;
        private Size m_szForm;
        private float m_fScale;

        private Rectangle m_rectSaveO;
        private Rectangle m_rectSaveC;

        private bool m_bOnSaveO;
        private bool m_bOnSaveC;
        private bool m_bOnClose;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Width = m_bmp.Width + 2;
            this.Height = m_bmp.Height + 2;
            m_szForm = m_bmp.Size;
            m_fScale = 1;
            this.Twist();
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) this.Close();
            if (e.Button == MouseButtons.Left)
            {
                if (e.X >= this.Width - 20 && e.Y <= 20) this.Close();
            }
            if (m_bOnSaveC) SaveBmp(false);
            if (m_bOnSaveO) SaveBmp(true);
            base.OnMouseClick(e);
        }

        protected override void OnDoubleClick(EventArgs e)
        {
            base.OnDoubleClick(e);
            if (this.WindowState == FormWindowState.Normal)
                this.WindowState = FormWindowState.Maximized;
            else
            {
                this.WindowState = FormWindowState.Normal;
                this.Twist();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            m_ptOriginal = e.Location;
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Middle)
                this.Location = (Point)((Size)MousePosition - (Size)m_ptOriginal);
            if (m_rectSaveO.Contains(e.Location))
            {
                if (!m_bOnSaveO)
                {
                    m_bOnSaveO = true;
                    this.Invalidate(m_bOnSaveO);
                }
            }
            else
            {
                if (m_bOnSaveO)
                {
                    m_bOnSaveO = false;
                    this.Invalidate(m_bOnSaveO);
                }
            }
            if (m_rectSaveC.Contains(e.Location))
            {
                if (!m_bOnSaveC)
                {
                    m_bOnSaveC = true;
                    this.Invalidate(m_rectSaveC);
                }
            }
            else
            {
                if (m_bOnSaveC)
                {
                    m_bOnSaveC = false;
                    this.Invalidate(m_rectSaveC);
                }
            }
            if (e.X >= this.Width - 20 && e.Y <= 20)
            {
                if (!m_bOnClose)
                {
                    m_bOnClose = true;
                    this.ContextMenuStrip = contextMenuStrip1;
                    this.Invalidate(new Rectangle(this.Width - 20, 1, 19, 19));
                }
            }
            else
            {
                if (m_bOnClose)
                {
                    m_bOnClose = false;
                    this.ContextMenuStrip = null;
                    this.Invalidate(new Rectangle(this.Width - 20, 1, 19, 19));
                }
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            m_bMouseEnter = true;
            this.Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            m_bOnSaveC = m_bOnSaveO = m_bMouseEnter = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            if (m_bMouseEnter)
            {
                float nIncrement = 0;
                if (e.Delta > 0)
                {
                    if (this.Width < Screen.PrimaryScreen.Bounds.Width
                        || this.Height < Screen.PrimaryScreen.Bounds.Height)
                        nIncrement = 0.1F;
                    else return;
                }
                if (e.Delta < 0)
                {
                    if (this.Width > 100 || this.Height > 30)
                        nIncrement = -0.1F;
                    else return;
                }

                m_fScale += nIncrement;
                if (!m_bMinimum && !m_bMaxmum)
                {
                    this.Left = (int)(MousePosition.X - (int)(e.X / (m_fScale - nIncrement)) * m_fScale);
                    this.Top = (int)(MousePosition.Y - (int)(e.Y / (m_fScale - nIncrement)) * m_fScale);
                }
                this.Width = (int)(m_szForm.Width * m_fScale + 2);
                this.Height = (int)(m_szForm.Height * m_fScale + 2);
            }
            base.OnMouseWheel(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (m_bmp == null)
            {
                MessageBox.Show("Bitmap cannot be null!");
                this.Close();
            }
            Graphics g = e.Graphics;
            g.DrawImage(m_bmp, 1, 1, this.Width - 2, this.Height - 2);
            g.DrawRectangle(Pens.Cyan, 0, 0, this.Width - 1, this.Height - 1);
            if (m_bMouseEnter || m_bLoad)
            {
                using (SolidBrush sb = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                {
                    g.FillRectangle(sb, 1, 1, this.Width - 2, this.Height - 2);
                    sb.Color = Color.FromArgb(150, 0, 255, 255);

                    StringFormat sf = new StringFormat();

                    string strDraw = "Original:\t[" + m_bmp.Width + "," + m_bmp.Height + "]"
                        + "\tScale:" + ((double)(this.Width - 2) / m_bmp.Width).ToString("F2") + "[W]"
                        + "\r\nCurrent:\t[" + (this.Width - 2) + "," + (this.Height - 2) + "]"
                        + "\tScale:" + ((double)(this.Height - 2) / m_bmp.Height).ToString("F2") + "[H]";
                    sf.SetTabStops(0.0F, new float[] { 60.0F, 60.0F });
                    Rectangle rectString = new Rectangle(new Point(1, 1), g.MeasureString(strDraw, this.Font, this.Width, sf).ToSize());
                    rectString.Inflate(1, 1);
                    g.FillRectangle(sb, rectString);
                    g.DrawString(strDraw, this.Font, Brushes.Wheat, rectString, sf);

                    rectString = new Rectangle(0, this.Height - 2 * this.Font.Height - 1,
                        this.Width, this.Font.Height * 2);
                    sf.Alignment = StringAlignment.Far;
                    g.FillRectangle(sb, rectString);
                    g.DrawString("Move [W,S,A,D] ReSize [T,G,F,H]\r\nScale [MouseWheel] Exit [MouseRight]", this.Font, Brushes.Wheat, rectString, sf);
                    g.DrawString("SaveOriginal\r\nSaveCurrent", this.Font, Brushes.Wheat, rectString.X + 12, rectString.Y);

                    g.FillRectangle(sb, this.Width - 21, 1, 20, 20);
                    if (m_bOnClose)
                        g.FillRectangle(Brushes.Red, this.Width - 20, 1, 19, 19);

                    sb.Color = m_bOnSaveO ? Color.Red : Color.Wheat;
                    m_rectSaveO = new Rectangle(2, rectString.Y + 2, 10, this.Font.Height - 3);
                    g.FillRectangle(sb, m_rectSaveO);
                    sb.Color = m_bOnSaveC ? Color.Red : Color.Wheat;
                    m_rectSaveC = new Rectangle(2, rectString.Y + this.Font.Height + 1, 10, this.Font.Height - 2);
                    g.FillRectangle(sb, m_rectSaveC);
                }
            }
            base.OnPaint(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w') this.Top -= 1;
            if (e.KeyChar == 's') this.Top += 1;
            if (e.KeyChar == 'a') this.Left -= 1;
            if (e.KeyChar == 'd') this.Left += 1;
            if (e.KeyChar == 't') m_szForm.Height = (int)(((this.Height -= 1) - 2) / m_fScale);
            if (e.KeyChar == 'g') m_szForm.Height = (int)(((this.Height += 1) - 2) / m_fScale);
            if (e.KeyChar == 'f') m_szForm.Width = (int)(((this.Width -= 1) - 2) / m_fScale);
            if (e.KeyChar == 'h') m_szForm.Width = (int)(((this.Width += 1) - 2) / m_fScale);
            base.OnKeyPress(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            int w = Screen.PrimaryScreen.Bounds.Width;
            int h = Screen.PrimaryScreen.Bounds.Height;
            if (width < 100) width = 100;
            if (width > Screen.PrimaryScreen.Bounds.Width) width = w;
            if (height < 30) height = 30;
            if (height > Screen.PrimaryScreen.Bounds.Height) height = h;
            m_bMinimum = width == 100 || height == 30;
            m_bMaxmum = width == w || height == h;
            if (m_bMaxmum) x = y = 0;
            base.SetBoundsCore(x, y, width, height, specified);
        }

        private void TMenuItem_OriginalToClip_Click(object sender, EventArgs e)
        {
            this.SetClipBoard(true);
        }

        private void TMenuItem_CurrentToClip_Click(object sender, EventArgs e)
        {
            this.SetClipBoard(false);
        }

        private void TMenuItem_SaveOriginal_Click(object sender, EventArgs e)
        {
            this.SaveBmp(true);
        }

        private void TMenuItem_SaveCurrent_Click(object sender, EventArgs e)
        {
            this.SaveBmp(false);
        }

        private void TMenuItem_Size_Click(object sender, EventArgs e)
        {
            FrmSize frmSize = new FrmSize(new Size(this.Width - 2, this.Height - 2));
            if (frmSize.ShowDialog() == DialogResult.OK)
            {
                this.Width = frmSize.ImageSize.Width + 2;
                this.Height = frmSize.ImageSize.Height + 2;
                m_szForm.Width = (int)((this.Width - 2) / m_fScale);
                m_szForm.Height = (int)((this.Height - 2) / m_fScale);
            }
        }

        private void TMenuItem_Help_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "MoveWindow\t[W,A,S,D],[MouseMiddle]\r\n\t\t[MouseDown and move]\r\n" +
                "ReSizeWindow\t[T,F,G,H],[MouseWheel]\r\n" +
                "WindowState\t[MouseDoubleClick]");
        }

        private void TMenuItem_Close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Twist()
        {
            Thread threadShow = new Thread(new ThreadStart(() =>
            {
                for (int i = 0; i < 4; i++)
                {
                    m_bLoad = !m_bLoad;
                    this.Invalidate();
                    Thread.Sleep(250);
                }
            }));
            threadShow.IsBackground = true;
            threadShow.Start();
        }

        private void SetClipBoard(bool bOriginal)
        {
            if (bOriginal)
            {
                Clipboard.SetImage(m_bmp);
                return;
            }
            using (Bitmap bmp = new Bitmap(this.Width - 2, this.Height - 2, PixelFormat.Format24bppRgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(m_bmp, 0, 0, bmp.Width, bmp.Height);
                    Clipboard.SetImage(bmp);
                }
            }
        }

        private void SaveBmp(bool bOriginal)
        {
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = "Bitmap(*.bmp)|*.bmp|JPEG(*.jpg)|*.jpg";
            saveDlg.FilterIndex = 2;
            saveDlg.FileName = "CAPTURE_" + GetTimeString();
            if (saveDlg.ShowDialog() == DialogResult.OK)
            {
                using (Bitmap bmp = bOriginal ? m_bmp.Clone() as Bitmap :
                    new Bitmap(this.Width - 2, this.Height - 2, PixelFormat.Format24bppRgb))
                {
                    if (bOriginal)
                    {
                        using (Graphics g = Graphics.FromImage(bmp))
                        {
                            g.DrawImage(m_bmp, 0, 0, this.Width - 2, this.Height - 2);
                        }
                    }
                    switch (saveDlg.FilterIndex)
                    {
                        case 1:
                            bmp.Save(saveDlg.FileName, ImageFormat.Bmp);
                            break;
                        case 2:
                            bmp.Save(saveDlg.FileName, ImageFormat.Jpeg);
                            break;
                    }
                }
            }
        }
        //保存时获取当前时间字符串作文默认文件名
        private string GetTimeString()
        {
            DateTime time = DateTime.Now;
            return time.Date.ToShortDateString().Replace("/", "") + "_" +
                time.ToLongTimeString().Replace(":", "");
        }
    }
}
