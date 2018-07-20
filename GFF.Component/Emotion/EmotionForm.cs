using CCWin;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using GFF.Helper;

namespace GFF.Component.Emotion
{
    public partial class EmotionForm : CCSkinMain
    {

        string imagedbPath;

        private void SetStyles()
        {
            SetStyle(
                ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.ResizeRedraw |
                ControlStyles.DoubleBuffer, true);
            //强制分配样式重新应用到控件上
            UpdateStyles();
        }

        public EmotionForm()
        {
            InitializeComponent();
            SetStyles();
            // Window Style
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Minimized;
            //this.Show();
            this.Hide();
            this.WindowState = FormWindowState.Normal;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
        }

        #region Protected Member Variables

        protected Bitmap _Bitmap;

        protected int _imageWidth;
        protected int _imageHeight;

        protected int _nBitmapWidth;
        protected int _nBitmapHeight;
        protected int _nItemWidth;
        protected int _nItemHeight;
        protected int _nRows;
        protected int _nColumns;
        protected int _nHSpace;
        protected int _nVSpace;
        protected int _nCoordX = -1;
        protected int _nCoordY = -1;
        protected bool _bIsMouseDown;

        protected int offset = 5;

        private bool IsShowDialog;
        private int PageIndex;
        private int PageCount;
        private int PageImageCount;
        private int imageIndex;

        #endregion

        #region Public Properties

        public Color BackgroundColor = Color.FromArgb(255, 255, 255);
        public Color BackgroundOverColor = Color.FromArgb(241, 238, 231);
        public Color HLinesColor = Color.FromArgb(222, 222, 222);
        public Color VLinesColor = Color.FromArgb(165, 182, 222);
        public Color BorderColor = Color.FromArgb(0, 16, 123);

        private PictureBox Demo;
        private IContainer components;

        public bool EnableDragDrop;

        private List<ImageEntity> _images = new List<ImageEntity>();

        public List<ImageEntity> Images
        {
            get { return this._images; }
            set { this._images = value; }
        }

        private List<ImageEntity> _drawImages = new List<ImageEntity>();

        public List<ImageEntity> DrawImages
        {
            get { return this._drawImages; }
            set { this._drawImages = value; }
        }

        private string _customImagePath = "";

        public string CustomImagePath
        {
            get { return this._customImagePath; }
            set { this._customImagePath = value; }
        }

        public bool CanManage { get; set; }

        private string _imagePath = "";

        private ContextMenuStrip ImageMenu;

        private ToolStripMenuItem DeleteItem;
        private ToolStripMenuItem AddItem;

        private RectangleF FaceRect { get; set; }
        private Rectangle ClientRect { get; set; }
        private RectangleF Rect { get; set; }

        private RectangleF PageInfoRect { get; set; }

        private RectangleF MemoRect { get; set; }

        private RectangleF PageUpRect { get; set; }
        private RectangleF PageDownRect { get; set; }

        private Color HoveColor { get; set; }

        private readonly StringFormat sf = new StringFormat
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center
        };

        public string ImagePath
        {
            get { return this._imagePath; }
            set { this._imagePath = value; }
        }

        public bool ShowDemo { get; set; }

        private bool HoveUp { get; set; }

        private bool HoveDown { get; set; }

        public bool IsCustomImage { get; set; }

        #endregion

        #region Events

        public event SelectFaceHnadler Selected = null;

        protected virtual void OnSelected(SelectFaceArgs e)
        {
            if (this.Selected != null)
            {
                this.Selected(this, e);
            }
        }

        #endregion


        #region Public Methods

        public bool Init(int imageWidth, int imageHeight, int nHSpace, int nVSpace, int nColumns, int nRows)
        {
            imagedbPath = Path.Combine(Application.StartupPath, this.ImagePath + "Images.db");

            if (this.DesignMode)
            {
                return false;
            }

            try
            {
                ImageEntity img;
                this.Images.Clear();
                this.DrawImages.Clear();
                try
                {
                    if (!File.Exists(imagedbPath))
                    {
                        List<string> listFace = new List<string>(Directory.GetFiles(Path.Combine(Application.StartupPath, this.ImagePath)));
                        listFace.Sort(new Comparison<string>(GlobalResourceManager.CompareEmotionName));
                        foreach (string imgPath in listFace)
                        {
                            img = new ImageEntity(imgPath);

                            if (img.Image != null)
                            {
                                this.Images.Add(img);
                            }
                        }
                        try
                        {
                            FileHelper.SerializeFile(imagedbPath, this.Images);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(ex);
                        }
                    }
                    else
                    {
                        this.Images = FileHelper.DeserializeFile<List<ImageEntity>>(imagedbPath);
                        try
                        {
                            foreach (var item in this.Images)
                            {
                                if (!File.Exists(item.FullName))
                                {
                                    item.IsDelete = true;
                                }
                            }

                        }
                        catch { }

                        foreach (var imgItem in this.Images.FindAll(item => item.IsDelete))
                        {
                            try
                            {
                                File.Delete(imgItem.FullName);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                    this.Images = new List<ImageEntity>();
                }

                this.DrawImages = this.Images.FindAll(item => !item.IsDelete);

                this.Images = this.DrawImages.FindAll(item => !item.IsDelete);

                this.Demo.Width = imageWidth * 3 - 10;

                this.Demo.Height = imageHeight * 3 - 10;

                this._nColumns = nColumns;
                this._nRows = nRows;
                this._nHSpace = nHSpace;
                this._nVSpace = nVSpace;

                this._imageWidth = imageWidth;
                this._imageHeight = imageHeight;
                this._nItemWidth = imageWidth + nHSpace;
                this._nItemHeight = imageHeight + nVSpace;

                this._nBitmapWidth = this._nColumns * this._nItemWidth + 1;
                this._nBitmapHeight = this._nRows * this._nItemHeight + 1;
                this.Width = this._nBitmapWidth;
                this.Height = this._nBitmapHeight + 20;

                using (Graphics g = this.CreateGraphics())
                {
                    this.PageIndex = 0;
                    this.PageImageCount = this._nColumns * this._nRows;

                    this.UpdatePageCount();

                    this.ClientRect = new Rectangle(0, 0, this.Width - 1, this.Height - 1);

                    this.FaceRect = new RectangleF(0f, 0f, this._nBitmapWidth, this._nBitmapHeight);

                    this.Rect = new RectangleF(1f, 1f, this._nBitmapWidth - 2, this._nBitmapHeight - 2);

                    SizeF s = g.MeasureString("上一页", this.Font);
                    SizeF i = g.MeasureString(string.Format("{0}/{1}", this.PageCount, this.PageCount), this.Font);
                    SizeF z = g.MeasureString("单击右键进行管理!", this.Font);

                    this.PageInfoRect = new RectangleF(new PointF(this.ClientRect.Width - s.Width * 2 - i.Width - 20, this.FaceRect.Height + 3), i);

                    this.PageDownRect = new RectangleF(new PointF(this.ClientRect.Width - s.Width - 10, this.FaceRect.Height + 3), s);

                    this.PageUpRect = new RectangleF(new PointF(this.ClientRect.Width - s.Width * 2 - 10, this.FaceRect.Height + 3), s);

                    this.MemoRect = new RectangleF(new PointF(6, this.FaceRect.Height + 3), z);

                    this.HoveColor = Color.Blue;

                    this.DrawBackImage();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
            return true;
        }

        private void UpdatePageCount()
        {
            float t = this.DrawImages.Count / (float)this.PageImageCount;

            this.PageCount = (t > (int)t) ? (int)t + 1 : (int)t;

            if (this.PageCount == 1)
            {
                this.PageIndex = 0;
            }
        }

        private void DrawBackImage()
        {
            this._Bitmap = new Bitmap(this._nBitmapWidth, this._nBitmapHeight);

            using (Graphics g = Graphics.FromImage(this._Bitmap))
            {
                g.FillRectangle(new SolidBrush(this.BackgroundColor), 0, 0, this._nBitmapWidth, this._nBitmapHeight);

                for (int i = 0; i < this._nColumns; i++)
                {
                    g.DrawLine(new Pen(this.VLinesColor), i * this._nItemWidth, 0, i * this._nItemWidth,
                               this._nBitmapHeight - 1);
                }
                for (int i = 0; i < this._nRows; i++)
                {
                    g.DrawLine(new Pen(this.HLinesColor), 0, i * this._nItemHeight, this._nBitmapWidth - 1,
                               i * this._nItemHeight);
                }

                g.DrawRectangle(new Pen(this.BorderColor), 0, 0, this._nBitmapWidth - 1, this._nBitmapHeight - 1);

                for (int i = 0; i < this._nColumns; i++)
                {
                    for (int j = 0; j < this._nRows; j++)
                    {
                        if ((j * this._nColumns + i) < this.DrawImages.Count - this.PageIndex * this.PageImageCount)
                        {
                            g.DrawImage(this.DrawImages[this.PageIndex * this.PageImageCount + j * this._nColumns + i].Image,
                                        i * this._nItemWidth + this._nHSpace / 2,
                                        j * this._nItemHeight + this._nVSpace / 2, this._imageWidth, this._imageHeight);
                        }
                    }
                }
            }
        }

        public void Show(int x, int y)
        {
            this.Show(x, y, 0);
        }

        public void Show(int x, int y, int offsetHeight)
        {
            this.Show(x, y, null, offsetHeight);
        }

        public void Show(int x, int y, Control ctl)
        {
            this.Show(x, y, ctl, 0);
        }

        public void Show(int x, int y, Control ctl, int offsetHeight)
        {
            Point pt = new Point(x, y);
            int tmpHeight = 0;
            if (ctl != null)
            {
                tmpHeight = ctl.Top + ctl.Height;
            }

            if (pt.X < 0)
            {
                pt = new Point(0, pt.Y);
            }
            if (pt.Y < 0)
            {
                pt = new Point(pt.X, 0);
            }
            if (pt.Y + this.Height > Screen.PrimaryScreen.WorkingArea.Height)
            {
                pt = new Point(pt.X, pt.Y - this.Height - tmpHeight - offsetHeight);
            }
            if (pt.X + this.Width > Screen.PrimaryScreen.WorkingArea.Width)
            {
                pt = new Point(pt.X - (pt.X + this.Width - Screen.PrimaryScreen.WorkingArea.Width), pt.Y);
            }

            this.Left = pt.X;
            this.Top = pt.Y;
            this.Demo.Visible = false;
            this.IsShowDialog = false;
            this.Show();
            this.Refresh();
        }

        #endregion

        #region Overrides

        protected override void OnMouseLeave(EventArgs ea)
        {
            base.OnMouseLeave(ea);
            this._nCoordX = -1;
            this._nCoordY = -1;
            this.Invalidate();
        }

        /// <summary>
        /// 停用
        /// </summary>
        /// <param name="ea"></param>
        protected override void OnDeactivate(EventArgs ea)
        {
            if (this.IsShowDialog)
            {
                return;
            }
            this.ImageMenu.Hide();
            this.Hide();
        }

        protected override void OnKeyDown(KeyEventArgs kea)
        {
            if (this.DrawImages == null || this.DrawImages.Count == 0)
            {
                return;
            }

            if (this._nCoordX == -1 || this._nCoordY == -1)
            {
                this._nCoordX = 0;
                this._nCoordY = 0;
                this.Invalidate();
            }
            else
            {
                switch (kea.KeyCode)
                {
                    case Keys.Down:
                        if (this._nCoordY < this._nRows - 1)
                        {
                            this._nCoordY++;
                            this.Invalidate();
                        }
                        break;
                    case Keys.Up:
                        if (this._nCoordY > 0)
                        {
                            this._nCoordY--;
                            this.Invalidate();
                        }
                        break;
                    case Keys.Right:
                        if (this._nCoordX < this._nColumns - 1)
                        {
                            this._nCoordX++;
                            this.Invalidate();
                        }
                        break;
                    case Keys.Left:
                        if (this._nCoordX > 0)
                        {
                            this._nCoordX--;
                            this.Invalidate();
                        }
                        break;
                    case Keys.Enter:
                    case Keys.Space:
                        this.imageIndex = this.PageIndex * this.PageImageCount + this._nCoordY * this._nColumns + this._nCoordX;
                        if (this.Selected != null && this.imageIndex >= 0 && this.imageIndex < this.DrawImages.Count)
                        {
                            this.OnSelected(new SelectFaceArgs(this.DrawImages[this.imageIndex]));
                            this._nCoordX = -1;
                            this._nCoordY = -1;
                            this.Hide();
                        }
                        break;
                    case Keys.Escape:
                        this._nCoordX = -1;
                        this._nCoordY = -1;
                        this.Hide();
                        break;
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            this.HoveUp = this.PageUpRect.Contains(e.Location);

            this.HoveDown = this.PageDownRect.Contains(e.Location);

            if (!this.Rect.Contains(e.Location))
            {
                this._nCoordX = -1;
                this._nCoordY = -1;
                base.OnMouseMove(e);
                return;
            }

            if (this.DrawImages == null || this.DrawImages.Count == 0)
            {
                return;
            }

            Image tmpImg;

            if (((e.X / this._nItemWidth) != this._nCoordX) || ((e.Y / this._nItemHeight) != this._nCoordY))
            {
                this._nCoordX = e.X / this._nItemWidth;
                this._nCoordY = e.Y / this._nItemHeight;
                this.imageIndex = this.PageIndex * this.PageImageCount + this._nCoordY * this._nColumns + this._nCoordX;

                if (this.imageIndex >= 0 && this.imageIndex < this.DrawImages.Count)
                {
                    this.IsShowDialog = false;

                    this.Demo.Visible = true && ShowDemo;

                    if (this._nCoordX <= 2)
                    {
                        this.Demo.Left = this.Width - this.Demo.Width - 5;
                    }
                    else if (this._nColumns - this._nCoordX <= 3)
                    {
                        this.Demo.Left = 5;
                    }
                    tmpImg = this.DrawImages[this.imageIndex].Image;

                    if (tmpImg != null)
                    {
                        if (tmpImg.Width > this.Demo.Width || tmpImg.Height > this.Demo.Height)
                        {
                            this.Demo.SizeMode = PictureBoxSizeMode.Zoom;
                        }
                        else
                        {
                            this.Demo.SizeMode = PictureBoxSizeMode.CenterImage;
                        }

                        this.Demo.Image = tmpImg;
                    }
                }
                else
                {
                    this.Demo.Visible = false;
                }
                this.Invalidate();
            }
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.Rect.Contains(e.Location))
            {
                if (e.Button == MouseButtons.Left)
                {
                    this._bIsMouseDown = true;
                    this.Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (this.Rect.Contains(e.Location))
            {
                if (e.Button == MouseButtons.Left)
                {
                    if (this.DrawImages == null)
                    {
                        return;
                    }
                    this._bIsMouseDown = false;

                    this.imageIndex = this.PageIndex * this.PageImageCount + this._nCoordY * this._nColumns + this._nCoordX;

                    if (this.Selected != null && this.imageIndex >= 0 && this.imageIndex < this.DrawImages.Count)
                    {
                        this.OnSelected(new SelectFaceArgs(this.DrawImages[this.imageIndex], imageIndex));

                        this.Hide();
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            if (this.Rect.Contains(e.Location))
            {
                if (CanManage && e.Button == MouseButtons.Right)
                {
                    if (this.imageIndex >= 0 && this.imageIndex < this.DrawImages.Count)
                    {
                        this.DeleteItem.Enabled = this.DrawImages[this.imageIndex].IsCustom;
                    }
                    else
                    {
                        this.DeleteItem.Enabled = false;
                    }
                    this.ImageMenu.Show(MousePosition);
                }
            }

            if (this.PageUpRect.Contains(e.Location))
            {
                if (this.PageIndex > 0)
                {
                    this.Demo.Visible = false;
                    this.PageIndex--;
                    this.DrawBackImage();
                    this.Invalidate();
                }
            }

            if (this.PageDownRect.Contains(e.Location))
            {
                if (this.PageIndex < this.PageCount - 1)
                {
                    this.Demo.Visible = false;
                    this.PageIndex++;
                    this.DrawBackImage();
                    this.Invalidate();
                }
            }
            base.OnMouseClick(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            g.PageUnit = GraphicsUnit.Pixel;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;

            g.Clear(Color.White);

            g.DrawRectangle(new Pen(this.BorderColor), this.ClientRect);

            using (Bitmap offscreenBitmap = new Bitmap(this._nBitmapWidth, this._nBitmapHeight))
            {
                using (Graphics offscreenGrfx = Graphics.FromImage(offscreenBitmap))
                {
                    offscreenGrfx.DrawImage(this._Bitmap, this.FaceRect);

                    if (this.DrawImages != null && this.DrawImages.Count > 0)
                    {
                        if (this._nCoordX != -1 && this._nCoordY != -1 &&
                            (this._nCoordY * this._nColumns + this._nCoordX) <
                            this.DrawImages.Count - this.PageIndex * this.PageImageCount)
                        {
                            offscreenGrfx.FillRectangle(new SolidBrush(this.BackgroundOverColor),
                                                        this._nCoordX * this._nItemWidth + 1,
                                                        this._nCoordY * this._nItemHeight + 1,
                                                        this._nItemWidth - 1, this._nItemHeight - 1);
                            if (this._bIsMouseDown)
                            {
                                offscreenGrfx.DrawImage(
                                    this.DrawImages[
                                        this.PageIndex * this.PageImageCount + this._nCoordY * this._nColumns + this._nCoordX].Image,
                                    this._nCoordX * this._nItemWidth + this._nHSpace / 2 + 1,
                                    this._nCoordY * this._nItemHeight + this._nVSpace / 2 + 1, this._imageWidth,
                                    this._imageHeight);
                            }
                            else
                            {
                                offscreenGrfx.DrawImage(
                                    this.DrawImages[
                                        this.PageIndex * this.PageImageCount + this._nCoordY * this._nColumns + this._nCoordX].Image,
                                    this._nCoordX * this._nItemWidth + this._nHSpace / 2,
                                    this._nCoordY * this._nItemHeight + this._nVSpace / 2, this._imageWidth,
                                    this._imageHeight);
                            }
                            offscreenGrfx.DrawRectangle(new Pen(this.BorderColor), this._nCoordX * this._nItemWidth,
                                                        this._nCoordY * this._nItemHeight, this._nItemWidth,
                                                        this._nItemHeight);
                        }
                    }
                }

                g.DrawImage(offscreenBitmap, this.FaceRect);

                if (CanManage)
                {
                    using (SolidBrush b = new SolidBrush(Color.Black))
                    {
                        g.DrawString("单击右键进行管理!", this.Font, b, this.MemoRect, this.sf);

                        g.DrawString(string.Format("{0}/{1}", this.PageIndex + 1, this.PageCount), this.Font, b,
                                     this.PageInfoRect, this.sf);
                    }
                }

                using (SolidBrush b = new SolidBrush(this.HoveUp ? this.HoveColor : Color.Black))
                {
                    g.DrawString("上一页", this.Font, b, this.PageUpRect, this.sf);
                }

                using (SolidBrush b = new SolidBrush(this.HoveDown ? this.HoveColor : Color.Black))
                {
                    g.DrawString("下一页", this.Font, b, this.PageDownRect, this.sf);
                }
            }
        }

        #endregion
        /// <summary>
        /// 添加自定义表情
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddItem_Click(object sender, EventArgs e)
        {
            try
            {
                this.Demo.Visible = false;
                this.IsShowDialog = true;
                int count = 0;
                int err = 0;
                OpenFileDialog dlg = new OpenFileDialog
                {
                    Filter = ("图像文件 (*.jpg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|" +
                              "JPEG 文件 (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                              "PNG 文件 (*.png)|*.png|" +
                              "GIF 文件 (*.gif)|*.gif|" +
                              "位图文件 (*.bmp)|*.bmp|" +
                              "所有文件 (*.*)|*.*"),
                    FilterIndex = 0,
                    Title = "选择头像",
                    RestoreDirectory = true,
                    Multiselect = true
                };

                string tmpImgMD5 = "";
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    foreach (string file in dlg.FileNames)
                    {
                        if (File.Exists(file))
                        {
                            try
                            {
                                string shortname = string.Format(this.CustomImagePath + "{0}{1}", DateTime.Now.Ticks,
                                                                 Path.GetExtension(file));
                                string fullname = Path.Combine(Application.StartupPath, shortname);

                                try
                                {
                                    Image.FromStream(new MemoryStream(File.ReadAllBytes(file)));
                                }
                                catch
                                {
                                    err++;
                                    break;
                                }

                                FileHelper.CreateDir(Path.GetDirectoryName(fullname));

                                if (!Path.GetDirectoryName(file).Equals(Path.GetDirectoryName(fullname)))
                                {
                                    tmpImgMD5 = EncryptionHelper.MD5Helper.GetFileMd5(file);

                                    if (this.DrawImages.Exists(item => item.MD5.ToLower() == tmpImgMD5.ToLower()))
                                    {
                                        count++;
                                        break;
                                    }
                                    File.Copy(file, fullname, true);
                                }

                                ImageEntity img = new ImageEntity(fullname) { IsCustom = true };

                                if (!this.Images.Exists(item => item.MD5.ToLower() == img.MD5.ToLower()))
                                {
                                    this.Images.Add(img);
                                }
                                else
                                {
                                    ImageEntity tmp = this.Images.Find(item => item.MD5.ToLower() == img.MD5.ToLower());
                                    if (tmp != null)
                                    {
                                        tmp.IsDelete = false;
                                    }
                                }

                                this.DrawImages.Clear();

                                this.DrawImages = this.Images.FindAll(item => !item.IsDelete);

                            }
                            catch
                            {

                            }

                        }
                    }
                    if (dlg.FileNames.Length > 1)
                    {
                        this.DrawBackImage();
                        this.Invalidate();
                        MessageBox.Show(this,
                                        string.Format("共选择{0}个表情，{1}个表情己存在，成功添加{2}个表情,添加失败{3}个表情！", dlg.FileNames.Length,
                                                      count, dlg.FileNames.Length - count - err, err), "提示信息",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        if (count > 0)
                        {
                            MessageBox.Show(this, "表情已存在，请重新选择！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        if (err > 0)
                        {
                            MessageBox.Show(this, "表情添加失败，请重新选择！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            return;
                        }
                        this.DrawBackImage();
                        this.Invalidate();
                        MessageBox.Show(this, "表情添加成功！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    try
                    {
                        FileHelper.SerializeFile(Path.Combine(Application.StartupPath, this.ImagePath + "Images.db"), this.Images);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex);
                    }
                    this.UpdatePageCount();
                    this.RaiseOnChanged();
                }
            }
            catch { }
            finally
            {
                this.IsShowDialog = false;
            }
        }

        private void DeleteItem_Click(object sender, EventArgs e)
        {
            this.IsShowDialog = true;
            try
            {
                if (MessageBox.Show(this, "请您确认删除?", "提示信息", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) ==
                    DialogResult.OK)
                {
                    ImageEntity img = this.Images.Find(item => item.MD5 == this.DrawImages[this.imageIndex].MD5);
                    if (img != null)
                    {
                        img.IsDelete = true;
                    }
                    var dels = this.Images.FindAll(item => item.IsDelete);
                    if (dels != null && dels.Count > 0)
                    {
                        foreach (var item in dels)
                        {
                            try
                            {
                                File.Delete(item.FullName);
                            }
                            catch { }
                        }
                    }
                    this.DrawImages.Clear();
                    this.DrawImages = this.Images.FindAll(item => !item.IsDelete);
                    this.UpdatePageCount();
                    this.DrawBackImage();
                    this.Invalidate();
                    this.RaiseOnChanged();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
            finally
            {
                this.IsShowDialog = false;
            }
        }

        /// <summary>
        /// 自定义表情事件
        /// </summary>
        public delegate void OnChangedHandler();
        /// <summary>
        /// 自定义表情事件
        /// </summary>
        public event OnChangedHandler OnChanged;
        protected void RaiseOnChanged()
        {
            if (this.OnChanged != null) this.OnChanged();
        }

    }

    public delegate void SelectFaceHnadler(object sender, SelectFaceArgs e);

    public class SelectFaceArgs : EventArgs
    {
        public SelectFaceArgs()
        {
        }

        public SelectFaceArgs(ImageEntity img)
            : this()
        {
            this.Img = img;
        }

        public SelectFaceArgs(ImageEntity img, int ImageIndex)
            : this(img)
        {
            this.ImageIndex = ImageIndex;
        }

        public ImageEntity Img { get; set; }

        public int ImageIndex { get; set; }
    }
}
