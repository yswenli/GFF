using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GFF.Helper.Win32;

namespace GFF.Component.Capture
{
    public partial class ImageProcessBox : Control
    {
        public ImageProcessBox()
        {
            InitializeComponent();
            InitMember();

            this.ForeColor = Color.White;
            this.BackColor = Color.Black;
            this.Dock = DockStyle.Fill;

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private void InitMember()
        {
            this.dotColor = Color.Yellow;
            this.lineColor = Color.Cyan;
            this.magnifySize = new Size(15, 15);
            this.magnifyTimes = 7;
            this.isDrawOperationDot = true;
            this.isSetClip = true;
            this.isShowInfo = true;
            this.autoSizeFromImage = true;
            this.canReset = true;
            m_pen = new Pen(lineColor, 1);
            m_sb = new SolidBrush(dotColor);
            this.selectedRectangle = new Rectangle();
            this.ClearDraw();
            m_rectDots = new Rectangle[8];
            for (int i = 0; i < 8; i++)
            {
                m_rectDots[i] = new Rectangle(-10, -10, 5, 5);
            }
        }
        //貌似析构函数不执行
        ~ImageProcessBox()
        {
            m_pen.Dispose();
            m_sb.Dispose();
            if (this.baseImage != null)
            {
                m_bmpDark.Dispose();
                this.baseImage.Dispose();
            }
        }

        public void DeleResource()
        {
            m_pen.Dispose();
            m_sb.Dispose();
            if (this.baseImage != null)
            {
                m_bmpDark.Dispose();
                this.baseImage.Dispose();
            }
        }

        #region Properties

        private Image baseImage;
        /// <summary>
        /// 获取或设置用于被操作的图像
        /// </summary>
        [Category("Custom"), Description("获取或设置用于被操作的图像")]
        public Image BaseImage
        {
            get { return baseImage; }
            set
            {
                baseImage = value;
                this.BuildBitmap();
            }
        }

        private Color dotColor;
        /// <summary>
        /// 获取或设置操作框点的颜色
        /// </summary>
        [Description("获取或设置操作框点的颜色")]
        [DefaultValue(typeof(Color), "Yellow"), Category("Custom")]
        public Color DotColor
        {
            get { return dotColor; }
            set { dotColor = value; }
        }

        private Color lineColor;
        /// <summary>
        /// 获取或设置操作框线条的颜色
        /// </summary>
        [Description("获取或设置操作框线条的颜色")]
        [DefaultValue(typeof(Color), "Cyan"), Category("Custom")]
        public Color LineColor
        {
            get { return lineColor; }
            set { lineColor = value; }
        }

        private Rectangle selectedRectangle;
        /// <summary>
        /// 获取当前选中的区域
        /// </summary>
        [Browsable(false)]
        public Rectangle SelectedRectangle
        {
            get
            {
                Rectangle rectTemp = selectedRectangle;
                rectTemp.Width++; rectTemp.Height++;
                return rectTemp;
            }
        }

        private Size magnifySize;
        /// <summary>
        /// 获取或设置放大图像的原图大小尺寸
        /// </summary>
        [Description("获取或设置放大图像的原图大小尺寸")]
        [DefaultValue(typeof(Size), "15,15"), Category("Custom")]
        public Size MagnifySize
        {
            get { return magnifySize; }
            set
            {
                magnifySize = value;
                if (magnifySize.Width < 5) magnifySize.Width = 5;
                if (magnifySize.Width > 20) magnifySize.Width = 20;
                if (magnifySize.Height < 5) magnifySize.Height = 5;
                if (magnifySize.Height > 20) magnifySize.Height = 20;
            }
        }

        private int magnifyTimes;
        /// <summary>
        /// 获取或设置图像放大的倍数
        /// </summary>
        [Description("获取或设置图像放大的倍数")]
        [DefaultValue(7), Category("Custom")]
        public int MagnifyTimes
        {
            get { return magnifyTimes; }
            set
            {
                magnifyTimes = value;
                if (magnifyTimes < 3) magnifyTimes = 3;
                if (magnifyTimes > 10) magnifyTimes = 10;
            }
        }

        private bool isDrawOperationDot;
        /// <summary>
        /// 获取或设置是否绘制操作框点
        /// </summary>
        [Description("获取或设置是否绘制操作框点")]
        [DefaultValue(true), Category("Custom")]
        public bool IsDrawOperationDot
        {
            get { return isDrawOperationDot; }
            set
            {
                if (value == isDrawOperationDot) return;
                isDrawOperationDot = value;
                this.Invalidate();
            }
        }

        private bool isSetClip;
        /// <summary>
        /// 获取或设置是否限制鼠标操作区域
        /// </summary>
        [Description("获取或设置是否限制鼠标操作区域")]
        [DefaultValue(true), Category("Custom")]
        public bool IsSetClip
        {
            get { return isSetClip; }
            set { isSetClip = value; }
        }

        private bool isShowInfo;
        /// <summary>
        /// 获取或设置是否绘制信息展示
        /// </summary>
        [Description("获取或设置是否绘制信息展示")]
        [DefaultValue(true), Category("Custom")]
        public bool IsShowInfo
        {
            get { return isShowInfo; }
            set { isShowInfo = value; }
        }

        private bool autoSizeFromImage;
        /// <summary>
        /// 获取或设置是否根据图像大小自动调整控件尺寸
        /// </summary>
        [Description("获取或设置是否根据图像大小自动调整控件尺寸")]
        [DefaultValue(true), Category("Custom")]
        public bool AutoSizeFromImage
        {
            get { return autoSizeFromImage; }
            set
            {
                if (value && this.baseImage != null)
                {
                    this.Width = this.baseImage.Width;
                    this.Height = this.baseImage.Height;
                }
                autoSizeFromImage = value;
            }
        }

        private bool isDrawed;
        /// <summary>
        /// 获取当前是否绘制的有区域
        /// </summary>
        [Browsable(false)]
        public bool IsDrawed
        {
            get { return isDrawed; }
        }

        private bool isStartDraw;
        /// <summary>
        /// 获取当前是否开始绘制
        /// </summary>
        [Browsable(false)]
        public bool IsStartDraw
        {
            get { return isStartDraw; }
        }

        private bool isMoving;
        /// <summary>
        /// 获取当前操作框是否正在移动
        /// </summary>
        [Browsable(false)]
        public bool IsMoving
        {
            get { return isMoving; }
        }

        private bool canReset;
        /// <summary>
        /// 获取或设置操作框是否锁定
        /// </summary>
        [Browsable(false)]
        public bool CanReset
        {
            get { return canReset; }
            set
            {
                canReset = value;
                if (!canReset) this.Cursor = Cursors.Default;
            }
        }
        #endregion

        #region Member variable

        private bool m_bMouseEnter;
        private bool m_bLockH;
        private bool m_bLockW;
        private Point m_ptOriginal;
        private Point m_ptCurrent;
        private Point m_ptTempStarPos;
        private Rectangle[] m_rectDots;
        private Rectangle m_rectClip;

        private Bitmap m_bmpDark;
        private Pen m_pen;
        private SolidBrush m_sb;

        #endregion

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {        //根据情况是否开始绘制操作框
                if (!this.IsDrawed || this.Cursor != Cursors.Default)
                {
                    m_rectClip = this.DisplayRectangle;
                    if (this.baseImage != null)
                    {
                        if (this.isSetClip)
                        {
                            if (e.X > this.baseImage.Width || e.Y > this.baseImage.Height) return;
                            m_rectClip.Intersect(new Rectangle(0, 0, this.baseImage.Width, this.baseImage.Height));
                        }
                    }
                    Cursor.Clip = RectangleToScreen(m_rectClip);
                    isStartDraw = true;
                    m_ptOriginal = e.Location;
                }
            }
            this.Focus();
            base.OnMouseDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            m_ptCurrent = e.Location;
            m_bMouseEnter = true;

            #region Process OperationBox

            if (isDrawed && this.canReset)
            {        //如果已经绘制 并且可以操作选区 判断操作类型
                this.SetCursorStyle(e.Location);
                if (isStartDraw && this.isDrawOperationDot)
                {
                    if (m_rectDots[0].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.X = this.selectedRectangle.Right;
                        m_ptOriginal.Y = this.selectedRectangle.Bottom;
                    }
                    else if (m_rectDots[1].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.Y = this.selectedRectangle.Bottom;
                        m_bLockW = true;
                    }
                    else if (m_rectDots[2].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.X = this.selectedRectangle.X;
                        m_ptOriginal.Y = this.selectedRectangle.Bottom;
                    }
                    else if (m_rectDots[3].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.X = this.selectedRectangle.Right;
                        m_bLockH = true;
                    }
                    else if (m_rectDots[4].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.X = this.selectedRectangle.X;
                        m_bLockH = true;
                    }
                    else if (m_rectDots[5].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.X = this.selectedRectangle.Right;
                        m_ptOriginal.Y = this.selectedRectangle.Y;
                    }
                    else if (m_rectDots[6].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal.Y = this.selectedRectangle.Y;
                        m_bLockW = true;
                    }
                    else if (m_rectDots[7].Contains(e.Location))
                    {
                        isDrawed = false;
                        m_ptOriginal = this.selectedRectangle.Location;
                    }
                    else if (this.selectedRectangle.Contains(e.Location))
                    {
                        isDrawed = false;
                        isMoving = true;
                    }
                }
                base.OnMouseMove(e);
                return;
            }

            #endregion

            #region Calculate the operationbox

            if (isStartDraw)
            {
                if (isMoving)
                {     //如果移动选区 只重置 location
                    //Point ptLast = this.selectedRectangle.Location;
                    this.selectedRectangle.X = m_ptTempStarPos.X + e.X - m_ptOriginal.X;
                    this.selectedRectangle.Y = m_ptTempStarPos.Y + e.Y - m_ptOriginal.Y;
                    if (this.selectedRectangle.X < 0) this.selectedRectangle.X = 0;
                    if (this.selectedRectangle.Y < 0) this.selectedRectangle.Y = 0;
                    if (this.selectedRectangle.Right > m_rectClip.Width) this.selectedRectangle.X = m_rectClip.Width - this.selectedRectangle.Width - 1;
                    if (this.selectedRectangle.Bottom > m_rectClip.Height) this.selectedRectangle.Y = m_rectClip.Height - this.selectedRectangle.Height - 1;
                    //if (this.Location == ptLast) return;
                }
                else
                {            //其他情况 判断是锁定高 还是锁定宽
                    if (Math.Abs(e.X - m_ptOriginal.X) > 1 || Math.Abs(e.Y - m_ptOriginal.Y) > 1)
                    {
                        if (!m_bLockW)
                        {
                            selectedRectangle.X = m_ptOriginal.X - e.X < 0 ? m_ptOriginal.X : e.X;
                            selectedRectangle.Width = Math.Abs(m_ptOriginal.X - e.X);
                        }
                        if (!m_bLockH)
                        {
                            selectedRectangle.Y = m_ptOriginal.Y - e.Y < 0 ? m_ptOriginal.Y : e.Y;
                            selectedRectangle.Height = Math.Abs(m_ptOriginal.Y - e.Y);
                        }
                    }
                }
                this.Invalidate();
            }

            #endregion
            //绘制放大信息
            //if (this.baseImage != null && !isDrawed && !isMoving && isShowInfo)
            //    this.Invalidate();
            base.OnMouseMove(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            m_bMouseEnter = false;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {        //如果绘制太小 则视为无效
                if (this.selectedRectangle.Width >= 4 && this.selectedRectangle.Height >= 4)
                    isDrawed = true;
                else
                    this.ClearDraw();
                isMoving = m_bLockH = m_bLockW = false; //取消锁定
                isStartDraw = false;
                m_ptTempStarPos = this.selectedRectangle.Location;
                Cursor.Clip = new Rectangle();
            } //else if (e.Button == MouseButtons.Right)
            //this.ClearDraw();
            this.Invalidate();
            base.OnMouseUp(e);
        }
        //响应四个按键实现精确移动
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == 'w')
                MouseAndKeyHelper.SetCursorPos(MousePosition.X, MousePosition.Y - 1);
            else if (e.KeyChar == 's')
                MouseAndKeyHelper.SetCursorPos(MousePosition.X, MousePosition.Y + 1);
            else if (e.KeyChar == 'a')
                MouseAndKeyHelper.SetCursorPos(MousePosition.X - 1, MousePosition.Y);
            else if (e.KeyChar == 'd')
                MouseAndKeyHelper.SetCursorPos(MousePosition.X + 1, MousePosition.Y);
            base.OnKeyPress(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            if (this.baseImage != null)
            {
                g.DrawImage(m_bmpDark, 0, 0);
                g.DrawImage(this.baseImage, this.selectedRectangle, this.selectedRectangle, GraphicsUnit.Pixel);
            }
            this.DrawOperationBox(g);
            if (this.baseImage != null && e != null && e.Graphics != null && !isDrawed && !isMoving && m_bMouseEnter && isShowInfo)
            {
                DrawInfo(e.Graphics);
            }
            base.OnPaint(e);
        }
        //绘制操作框
        protected virtual void DrawOperationBox(Graphics g)
        {

            #region Draw SizeInfo

            string strDrawSize = "X:" + this.selectedRectangle.X + " Y:" + this.selectedRectangle.Y +
                " W:" + (this.selectedRectangle.Width + 1) + " H:" + (this.selectedRectangle.Height + 1);
            Size seStr = TextRenderer.MeasureText(strDrawSize, this.Font);
            int tempX = this.selectedRectangle.X;
            int tempY = this.selectedRectangle.Y - seStr.Height - 5;
            if (!m_rectClip.IsEmpty)
                if (tempX + seStr.Width >= m_rectClip.Right) tempX -= seStr.Width;
            if (tempY <= 0) tempY += seStr.Height + 10;

            m_sb.Color = Color.FromArgb(125, 0, 0, 0);
            g.FillRectangle(m_sb, tempX, tempY, seStr.Width, seStr.Height);
            m_sb.Color = this.ForeColor;
            g.DrawString(strDrawSize, this.Font, m_sb, tempX, tempY);

            #endregion

            if (!this.isDrawOperationDot)
            {
                m_pen.Width = 3; m_pen.Color = this.lineColor;
                g.DrawRectangle(m_pen, this.selectedRectangle);
                return;
            }
            //计算八个顶点位置
            m_rectDots[0].Y = m_rectDots[1].Y = m_rectDots[2].Y = this.selectedRectangle.Y - 2;
            m_rectDots[5].Y = m_rectDots[6].Y = m_rectDots[7].Y = this.selectedRectangle.Bottom - 2;
            m_rectDots[0].X = m_rectDots[3].X = m_rectDots[5].X = this.selectedRectangle.X - 2;
            m_rectDots[2].X = m_rectDots[4].X = m_rectDots[7].X = this.selectedRectangle.Right - 2;
            m_rectDots[3].Y = m_rectDots[4].Y = this.selectedRectangle.Y + this.selectedRectangle.Height / 2 - 2;
            m_rectDots[1].X = m_rectDots[6].X = this.selectedRectangle.X + this.selectedRectangle.Width / 2 - 2;
            m_pen.Width = 1; m_pen.Color = this.lineColor;
            g.DrawRectangle(m_pen, this.selectedRectangle);
            m_sb.Color = this.dotColor;
            foreach (Rectangle rect in m_rectDots)
            {
                g.FillRectangle(m_sb, rect);
            }
            if (this.selectedRectangle.Width <= 10 || this.selectedRectangle.Height <= 10)
                g.DrawRectangle(m_pen, this.selectedRectangle);
        }
        //绘制图像放大信息
        protected virtual void DrawInfo(Graphics g)
        {

            #region Calculate point

            int tempX = (m_ptCurrent.X > this.ClientRectangle.Width - 2 ? this.ClientRectangle.Width - 2 : m_ptCurrent.X) + 20;
            int tempY = m_ptCurrent.Y + 20;
            int tempW = this.magnifySize.Width * this.magnifyTimes + 8;
            int tempH = this.magnifySize.Width * this.magnifyTimes + 12 + this.Font.Height * 3;
            if (!m_rectClip.IsEmpty)
            {
                if (tempX + tempW >= this.m_rectClip.Right) tempX -= tempW + 30;
                if (tempY + tempH >= this.m_rectClip.Bottom) tempY -= tempH + 30;
            }
            else
            {
                if (tempX + tempW >= this.ClientRectangle.Width - 2) tempX -= tempW + 30;
                if (tempY + tempH >= this.ClientRectangle.Height) tempY -= tempH + 30;
            }
            Rectangle tempRectBorder = new Rectangle(tempX + 2, tempY + 2, tempW - 4, this.magnifySize.Width * this.magnifyTimes + 4);

            #endregion

            m_sb.Color = Color.FromArgb(200, 0, 0, 0);
            g.FillRectangle(m_sb, tempX, tempY, tempW, tempH);
            m_pen.Width = 2; m_pen.Color = Color.White;
            g.DrawRectangle(m_pen, tempRectBorder);

            #region Draw the magnified image

            using (Bitmap bmpSrc = new Bitmap(this.magnifySize.Width, this.magnifySize.Height, PixelFormat.Format32bppArgb))
            {
                using (Graphics gp = Graphics.FromImage(bmpSrc))
                {
                    gp.DrawImage(this.baseImage, -(m_ptCurrent.X - this.magnifySize.Width / 2), -(m_ptCurrent.Y - this.magnifySize.Height / 2));
                }
                using (Bitmap bmpInfo = ImageProcessBox.MagnifyImage(bmpSrc, this.magnifyTimes))
                {
                    g.DrawImage(bmpInfo, tempX + 4, tempY + 4);
                }
            }

            #endregion

            m_pen.Width = this.magnifyTimes - 2;
            m_pen.Color = Color.FromArgb(125, 0, 255, 255);
            int tempCenterX = tempX + (tempW + (this.magnifySize.Width % 2 == 0 ? this.magnifyTimes : 0)) / 2;
            int tempCenterY = tempY + 2 + (tempRectBorder.Height + (this.MagnifySize.Height % 2 == 0 ? this.magnifyTimes : 0)) / 2;
            g.DrawLine(m_pen, tempCenterX, tempY + 4, tempCenterX, tempRectBorder.Bottom - 2);
            g.DrawLine(m_pen, tempX + 4, tempCenterY, tempX + tempW - 4, tempCenterY);

            #region Draw Info

            m_sb.Color = this.ForeColor;
            Color clr = ((Bitmap)this.baseImage).GetPixel((m_ptCurrent.X >= this.ClientRectangle.Width - 2 ? this.ClientRectangle.Width - 2 : m_ptCurrent.X), m_ptCurrent.Y);
            g.DrawString("Size: " + (this.selectedRectangle.Width + 1) + " x "
                + (this.selectedRectangle.Height + 1),
                this.Font, m_sb, tempX + 2, tempRectBorder.Bottom + 2);
            g.DrawString(clr.A + "," + clr.R + "," + clr.G + "," + clr.B,
                this.Font, m_sb, tempX + 2, tempRectBorder.Bottom + 2 + this.Font.Height);
            g.DrawString("0x" + clr.A.ToString("X").PadLeft(2, '0') +
                clr.R.ToString("X").PadLeft(2, '0') +
                clr.G.ToString("X").PadLeft(2, '0') +
                clr.B.ToString("X").PadLeft(2, '0'),
                this.Font, m_sb, tempX + 2, tempRectBorder.Bottom + 2 + this.Font.Height * 2);
            m_sb.Color = clr;
            g.FillRectangle(m_sb, tempX + tempW - 2 - this.Font.Height,         //右下角颜色
                tempY + tempH - 2 - this.Font.Height,
                this.Font.Height,
                this.Font.Height);
            g.DrawRectangle(Pens.Cyan, tempX + tempW - 2 - this.Font.Height,    //右下角颜色边框
                tempY + tempH - 2 - this.Font.Height,
                this.Font.Height,
                this.Font.Height);
            g.FillRectangle(m_sb, tempCenterX - this.magnifyTimes / 2,          //十字架中间颜色
                tempCenterY - this.magnifyTimes / 2,
                this.magnifyTimes,
                this.magnifyTimes);
            g.DrawRectangle(Pens.Cyan, tempCenterX - this.magnifyTimes / 2,     //十字架中间边框
                tempCenterY - this.magnifyTimes / 2,
                this.magnifyTimes - 1,
                this.magnifyTimes - 1);

            #endregion
        }
        //放大图形
        private static Bitmap MagnifyImage(Bitmap bmpSrc, int times)
        {
            Bitmap bmpNew = new Bitmap(bmpSrc.Width * times, bmpSrc.Height * times, PixelFormat.Format32bppArgb);
            BitmapData bmpSrcData = bmpSrc.LockBits(new Rectangle(0, 0, bmpSrc.Width, bmpSrc.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData bmpNewData = bmpNew.LockBits(new Rectangle(0, 0, bmpNew.Width, bmpNew.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            byte[] bySrcData = new byte[bmpSrcData.Height * bmpSrcData.Stride];
            Marshal.Copy(bmpSrcData.Scan0, bySrcData, 0, bySrcData.Length);
            byte[] byNewData = new byte[bmpNewData.Height * bmpNewData.Stride];
            Marshal.Copy(bmpNewData.Scan0, byNewData, 0, byNewData.Length);
            for (int y = 0, lenY = bmpSrc.Height; y < lenY; y++)
            {
                for (int x = 0, lenX = bmpSrc.Width; x < lenX; x++)
                {
                    for (int cy = 0; cy < times; cy++)
                    {
                        for (int cx = 0; cx < times; cx++)
                        {
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride)] = bySrcData[x * 4 + y * bmpSrcData.Stride];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 1] = bySrcData[x * 4 + y * bmpSrcData.Stride + 1];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 2] = bySrcData[x * 4 + y * bmpSrcData.Stride + 2];
                            byNewData[(x * times + cx) * 4 + ((y * times + cy) * bmpNewData.Stride) + 3] = bySrcData[x * 4 + y * bmpSrcData.Stride + 3];
                        }
                    }
                }
            }
            Marshal.Copy(byNewData, 0, bmpNewData.Scan0, byNewData.Length);
            bmpSrc.UnlockBits(bmpSrcData);
            bmpNew.UnlockBits(bmpNewData);
            return bmpNew;
        }
        //设置鼠标指针样式
        private void SetCursorStyle(Point loc)
        {
            if (m_rectDots[0].Contains(loc) || m_rectDots[7].Contains(loc))
                this.Cursor = Cursors.SizeNWSE;
            else if (m_rectDots[1].Contains(loc) || m_rectDots[6].Contains(loc))
                this.Cursor = Cursors.SizeNS;
            else if (m_rectDots[2].Contains(loc) || m_rectDots[5].Contains(loc))
                this.Cursor = Cursors.SizeNESW;
            else if (m_rectDots[3].Contains(loc) || m_rectDots[4].Contains(loc))
                this.Cursor = Cursors.SizeWE;
            else if (this.selectedRectangle.Contains(loc) /*&& this.canReset*/)
                this.Cursor = Cursors.SizeAll;
            else
                this.Cursor = Cursors.Default;
        }

        private void BuildBitmap()
        {
            if (this.baseImage == null) return;
            m_bmpDark = new Bitmap(this.baseImage);
            using (Graphics g = Graphics.FromImage(m_bmpDark))
            {
                SolidBrush sb = new SolidBrush(Color.FromArgb(125, 0, 0, 0));
                g.FillRectangle(sb, 0, 0, m_bmpDark.Width, m_bmpDark.Height);
                sb.Dispose();
            }
        }
        /// <summary>
        /// 清空所有操作
        /// </summary>
        public void ClearDraw()
        {
            isDrawed = false;
            this.selectedRectangle.X = this.selectedRectangle.Y = -100;
            this.selectedRectangle.Width = this.selectedRectangle.Height = 0;
            this.Cursor = Cursors.Default;
            this.Invalidate();
        }
        /// <summary>
        /// 手动设置一个块选中区域
        /// </summary>
        /// <param name="rect">要选中区域</param>
        public void SetSelectRect(Rectangle rect)
        {
            rect.Intersect(this.DisplayRectangle);
            if (rect.IsEmpty) return;
            rect.Width--; rect.Height--;
            if (this.selectedRectangle == rect) return;
            this.selectedRectangle = rect;
            this.Invalidate();
        }
        /// <summary>
        /// 手动设置一个块选中区域
        /// </summary>
        /// <param name="pt">要选中区域的坐标</param>
        /// <param name="se">要选中区域的大小</param>
        public void SetSelectRect(Point pt, Size se)
        {
            Rectangle rectTemp = new Rectangle(pt, se);
            rectTemp.Intersect(this.DisplayRectangle);
            if (rectTemp.IsEmpty) return;
            rectTemp.Width--; rectTemp.Height--;
            if (this.selectedRectangle == rectTemp) return;
            this.selectedRectangle = rectTemp;
            this.Invalidate();
        }
        /// <summary>
        /// 手动设置一个块选中区域
        /// </summary>
        /// <param name="x">要选中区域的x坐标</param>
        /// <param name="y">要选中区域的y坐标</param>
        /// <param name="w">要选中区域的宽度</param>
        /// <param name="h">要选中区域的高度</param>
        public void SetSelectRect(int x, int y, int w, int h)
        {
            Rectangle rectTemp = new Rectangle(x, y, w, h);
            rectTemp.Intersect(this.DisplayRectangle);
            if (rectTemp.IsEmpty) return;
            rectTemp.Width--; rectTemp.Height--;
            if (this.selectedRectangle == rectTemp) return;
            this.selectedRectangle = rectTemp;
            this.Invalidate();
        }
        /// <summary>
        /// 手动设置信息显示的位置
        /// </summary>
        /// <param name="pt">要显示的位置</param>
        public void SetInfoPoint(Point pt)
        {
            if (m_ptCurrent == pt) return;
            m_ptCurrent = pt;
            m_bMouseEnter = true;
            this.Invalidate();
        }
        /// <summary>
        /// 手动设置信息显示的位置
        /// </summary>
        /// <param name="x">要显示位置的x坐标</param>
        /// <param name="y">要显示位置的y坐标</param>
        public void SetInfoPoint(int x, int y)
        {
            if (m_ptCurrent.X == x && m_ptCurrent.Y == y) return;
            m_ptCurrent.X = x;
            m_ptCurrent.Y = y;
            m_bMouseEnter = true;
            this.Invalidate();
        }
        /// <summary>
        /// 获取操作框内的图像
        /// </summary>
        /// <returns>结果图像</returns>
        public Bitmap GetResultBmp()
        {
            if (this.baseImage == null) return null;
            Bitmap bmp = new Bitmap(this.selectedRectangle.Width + 1, this.selectedRectangle.Height + 1);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(this.baseImage, -this.selectedRectangle.X, -this.selectedRectangle.Y);
            }
            //Bitmap bmp = ((Bitmap)this.baseImage).Clone(this.selectedRectangle, this.baseImage.PixelFormat);
            return bmp;
        }
    }
}
