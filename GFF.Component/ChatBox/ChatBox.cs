using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using GFF.Component.ChatBox.Internals;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Drawing.Imaging;
using GFF.Component;
using CCWin.SkinControl;

namespace GFF.Component.ChatBox
{
    public enum ChatBoxContextMenuMode
    {
        None = 0,
        ForInput,
        ForOutput
    }

    /// <summary>
    /// 支持图片和动画的RichTextBox。
    /// </summary>
    public class ChatBox : RichTextBox
    {
        private Dictionary<uint, Image> defaultEmotionDictionary = new Dictionary<uint, Image>();
        private ContextMenuStrip contextMenuStrip1;
        private IContainer components;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5; //表情图片在内置列表中的index - emotion
        private Image imageOnRightClick = null;
        private ToolStripMenuItem toolStripMenuItem6;
        private ContextMenuStrip contextMenuStrip3;
        private ToolStripMenuItem toolStripMenuItem7;

        /// <summary>
        /// 当文件（夹）拖放到控件内时，触发此事件。参数：文件路径的集合。
        /// </summary>
        [Description("当文件（夹）拖放到控件内时，触发此事件。")]
        public event SkinChatRichTextBox.CbGeneric<string[]> FileOrFolderDragDrop;      

        #region ContextMenuMode
         private ChatBoxContextMenuMode contextMenuMode = ChatBoxContextMenuMode.None;
         private ContextMenuStrip contextMenuStrip4;
         private ToolStripMenuItem toolStripMenuItem8;
         private ToolStripMenuItem toolStripMenuItem9;
         /// <summary>
         /// 快捷菜单的模式。
         /// </summary>
         [Description("快捷菜单的模式。")]
         public ChatBoxContextMenuMode ContextMenuMode
         {
             get { return contextMenuMode; }
             set { contextMenuMode = value; }
         } 
         #endregion

        #region PopoutImageWhenDoubleClick
         private bool popoutImageWhenDoubleClick = false;
         /// <summary>
         /// 双击图片时，是否弹出图片。
         /// </summary>
         public bool PopoutImageWhenDoubleClick
         {
             get { return popoutImageWhenDoubleClick; }
             set { popoutImageWhenDoubleClick = value; }
         } 
         #endregion

        public ChatBox()
        {
            this.InitializeComponent();            

            this.AllowDrop = false;
            this.DragDrop += new DragEventHandler(textBoxSend_DragDrop);
            this.DragEnter += new DragEventHandler(textBoxSend_DragEnter);        
            this.KeyDown += new KeyEventHandler(ChatBox_KeyDown);
            this.MouseDown += new MouseEventHandler(ChatBox_MouseDown);

            this.SizeChanged += new EventHandler(ChatBox_SizeChanged);           
            this.DoubleClick += new EventHandler(ChatBox_DoubleClick);
            this.LinkClicked += new LinkClickedEventHandler(ChatBox_LinkClicked);
        }

        private GifBox HitTest(Point pt ,bool selectTarget)
        {
            int index = this.GetCharIndexFromPosition(pt);
            Point origin = this.GetPositionFromCharIndex(index);
            GifBox box = null;
            bool backOne = false;
            List<REOBJECT> list = this.RichEditOle.GetAllREOBJECT();
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].posistion == index || list[i].posistion + 1 == index)
                {
                    box = (GifBox)Marshal.GetObjectForIUnknown(list[i].poleobj);
                    if (list[i].posistion + 1 == index)
                    {
                        origin = new Point(origin.X - box.Width, origin.Y);
                        backOne = true;
                    }
                    break;
                }
            }

            if (box == null)
            {
                return null;
            }

            Rectangle rect = new Rectangle(origin.X, origin.Y, box.Width, box.Height);
            if (!rect.Contains(pt))
            {               
                return null;
            }

            if (selectTarget)
            {
                this.Select(backOne ? index - 1 : index, 1);
            }

            return box;
        }

        void ChatBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != System.Windows.Forms.MouseButtons.Right)
            {
                return;
            }            

            if (this.contextMenuMode == ChatBoxContextMenuMode.None)
            {
                this.ContextMenuStrip = null;
                return;
            }

            if (this.contextMenuMode == ChatBoxContextMenuMode.ForInput)
            {
                this.toolStripMenuItem9.Visible = !string.IsNullOrEmpty(this.SelectedText);
                this.ContextMenuStrip = this.contextMenuStrip1;
                return;
            }            
        
            GifBox box = this.HitTest(e.Location ,true);           
            if (box == null)
            {
                if (!string.IsNullOrEmpty(this.SelectedText))
                {
                    this.imageOnRightClick = null;
                    this.ContextMenuStrip = this.contextMenuStrip4;
                    return;
                }
                this.imageOnRightClick = null;
                this.ContextMenuStrip = this.contextMenuStrip3;
                return;
            }          
            this.imageOnRightClick = box.Image;
            this.ContextMenuStrip = this.contextMenuStrip2;      
        }

        void ChatBox_LinkClicked(object sender, LinkClickedEventArgs e)
        {           
            System.Diagnostics.Process.Start(e.LinkText); 
        }

        #region ChatBox_KeyDown
        void ChatBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.V)
            {
                if (Clipboard.ContainsImage())
                {
                    Image img = Clipboard.GetImage();
                    if (img != null)
                    {
                        this.InsertImage(img);
                    }
                    e.Handled = true;
                    return;
                }
            }
        } 
        #endregion

        #region ChatBox_SizeChanged
        void ChatBox_SizeChanged(object sender, EventArgs e)
        {
            if (this.RichEditOle == null)
            {
                return;
            }

            List<REOBJECT> list = this.RichEditOle.GetAllREOBJECT();
            for (int i = 0; i < list.Count; i++)
            {
                GifBox box = (GifBox)Marshal.GetObjectForIUnknown(list[i].poleobj);
                box.Size = this.ComputeGifBoxSize(box.Image.Size);
            }
        } 
        #endregion

        #region ChatBox_DoubleClick
        void ChatBox_DoubleClick(object sender, EventArgs arg)
        {
            try
            {
                if (!this.popoutImageWhenDoubleClick)
                {
                    return;
                }

                MouseEventArgs e = arg as MouseEventArgs;
                if (e == null)
                {
                    return;
                }

                GifBox box = this.HitTest(e.Location, true);
                if (box == null)
                {
                    return;
                }

                ImageForm form = new ImageForm(box.Image);
                form.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private string GetPathToSave(string title, string defaultName, string iniDir)
        {
            string extendName = Path.GetExtension(defaultName);
            SaveFileDialog saveDlg = new SaveFileDialog();
            saveDlg.Filter = string.Format("The Files (*{0})|*{0}", extendName);
            saveDlg.FileName = defaultName;
            saveDlg.InitialDirectory = iniDir;
            saveDlg.OverwritePrompt = false;
            if (title != null)
            {
                saveDlg.Title = title;
            }

            DialogResult res = saveDlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                return saveDlg.FileName;
            }

            return null;
        }
        #endregion

        #region textBoxSend_DragEnter
        void textBoxSend_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        } 
        #endregion

        #region textBoxSend_DragDrop
        void textBoxSend_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] fileOrDirs = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (fileOrDirs == null || fileOrDirs.Length == 0)
                {
                    return;
                }

                if (this.FileOrFolderDragDrop != null)
                {
                    this.FileOrFolderDragDrop(fileOrDirs);
                }
            }
        } 
        #endregion        

        #region RichEditOle
        private RichEditOle richEditOle;
        private RichEditOle RichEditOle
        {
            get
            {
                if (richEditOle == null)
                {
                    if (base.IsHandleCreated)
                    {
                        richEditOle = new RichEditOle(this);
                    }
                }

                return richEditOle;
            }
        } 
        #endregion     

        #region Initialize
        public void Initialize(Dictionary<uint, Image> defaultEmotions)
        {
            this.defaultEmotionDictionary = defaultEmotions ?? new Dictionary<uint, Image>();
        } 
        #endregion                         

        #region InsertImage 、InsertDefaultEmotion
        public void InsertDefaultEmotion(uint emotionID)
        {
            this.InsertDefaultEmotion(emotionID, this.TextLength);
        }

        /// <summary>
        /// 在position位置处，插入系统内置表情。
        /// </summary>      
        /// <param name="position">插入的位置</param>
        /// <param name="emotionID">表情图片在内置列表中的index</param>
        public void InsertDefaultEmotion(uint emotionID, int position)
        {
            try
            {
                Image image = this.defaultEmotionDictionary[emotionID];
                GifBox gif = new GifBox();
                gif.Cursor = Cursors.Hand;
                gif.BackColor = base.BackColor;
                gif.Size = this.ComputeGifBoxSize(image.Size);
                gif.Image = image;
                this.RichEditOle.InsertControl(gif, position, emotionID);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            } 
        }

        public void InsertImage(Image image)
        {
            this.InsertImage(image, this.TextLength);
        }

        /// <summary>
        /// 在position位置处，插入图片。
        /// </summary>   
        /// <param name="image">要插入的图片</param>
        /// <param name="position">插入的位置</param>       
        public void InsertImage(Image image, int position)
        {
            try
            {
                GifBox gif = new GifBox();
                gif.Cursor = Cursors.Hand;
                gif.BackColor = base.BackColor;
                gif.Size = this.ComputeGifBoxSize(image.Size);
                gif.Image = image;
                this.RichEditOle.InsertControl(gif, position, 10000);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            } 
        }

        private Size ComputeGifBoxSize(Size imgSize)
        {
            int maxWidth = this.Width - 20;

            if (imgSize.Width <= maxWidth)
            {
                return imgSize;
            }

            int newImgHeight = maxWidth * imgSize.Height / imgSize.Width; ;
            return new Size(maxWidth, newImgHeight);
        }
        #endregion

        #region AppendRtf
        public void AppendRtf(string _rtf)
        {
            try
            {
                base.Select(this.TextLength, 0);
                base.SelectedRtf = _rtf;
                base.Update();
                base.Select(this.Rtf.Length, 0);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            } 
        } 
        #endregion

        #region GetContent
        /// <summary>
        /// 获取Box中的所有内容。
        /// </summary>        
        /// <param name="containsForeignObject">内容中是否包含不是由IImagePathGetter管理的图片对象</param>
        /// <returns>key为位置，val为图片的ID</returns>
        public ChatBoxContent GetContent()
        {
            ChatBoxContent content = new ChatBoxContent(this.Text ,this.Font,this.ForeColor);            
            List<REOBJECT> list = this.RichEditOle.GetAllREOBJECT();
            for (int i = 0; i < list.Count; i++)
            {
                uint pos = (uint)list[i].posistion ;
                content.PicturePositions.Add(pos) ;
                if (list[i].dwUser != 10000)
                {
                    content.AddEmotion(pos, list[i].dwUser);
                }
                else
                {
                    GifBox box = (GifBox)Marshal.GetObjectForIUnknown(list[i].poleobj);
                    content.AddForeignImage(pos, box.Image);
                }
            }

            return content;
        }
        #endregion

        #region AppendRichText
        /// <summary>
        /// 在现有内容后面追加富文本。
        /// </summary>      
        public void AppendRichText(string textContent, Font font ,Color color)
        {
            try
            {
                int count = this.Text.Length;
                this.AppendText(textContent);

                this.Select(count, textContent.Length);
                if (color != null)
                {
                    this.SelectionColor = color;
                }
                if (font != null)
                {
                    this.SelectionFont = font;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            } 
        }       
        #endregion       

        #region AppendChatBoxContent
        public void AppendChatBoxContent(ChatBoxContent content)
        {
            try
            {
                if (content == null || content.Text == null)
                {
                    return;
                }

                int count = this.Text.Length;
                if (content.EmotionDictionary != null)
                {
                    string pureText = content.Text;
                    //去掉表情和图片的占位符
                    List<uint> emotionPosList = new List<uint>(content.EmotionDictionary.Keys);
                    List<uint> tempList = new List<uint>();
                    tempList.AddRange(emotionPosList);
                    foreach (uint key in content.ForeignImageDictionary.Keys)
                    {
                        tempList.Add(key);
                    }
                    tempList.Sort();

                    for (int i = tempList.Count - 1; i >= 0; i--)
                    {
                        pureText = pureText.Remove((int)tempList[i], 1);
                    }
                    this.AppendText(pureText);
                    //插入表情
                    for (int i = 0; i < tempList.Count; i++)
                    {
                        uint position = tempList[i];
                        if (emotionPosList.Contains(position))
                        {
                            this.InsertDefaultEmotion(content.EmotionDictionary[position], (int)(count + position));
                        }
                        else
                        {
                            this.InsertImage(content.ForeignImageDictionary[position], (int)(count + position));
                        }
                    }
                }
                else
                {
                    this.AppendText(content.Text);
                }

                this.Select(count, content.Text.Length);
                if (content.Color != null)
                {
                    this.SelectionColor = content.Color;
                }
                if (content.Font != null)
                {
                    this.SelectionFont = content.Font;
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            } 
        } 
        #endregion       

        #region Clear
        public new void Clear()
        {
            try
            {
                List<REOBJECT> list = this.RichEditOle.GetAllREOBJECT();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].dwUser == 10000)
                    {
                        GifBox box = (GifBox)Marshal.GetObjectForIUnknown(list[i].poleobj);
                        box.Dispose();
                    }
                }
                base.Clear();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }            
        }     
        #endregion     

        #region InitializeComponent
        private void InitializeComponent()
        {            
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem9 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.contextMenuStrip3.SuspendLayout();
            this.contextMenuStrip4.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem9,
            this.toolStripMenuItem2,
            this.toolStripMenuItem1});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem2.Text = "粘贴";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem1.Text = "插入图片";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(137, 92);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem3.Text = "复制";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.toolStripMenuItem3_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem4.Text = "另存为";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem5.Text = "新窗口显示";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(136, 22);
            this.toolStripMenuItem6.Text = "清屏";
            this.toolStripMenuItem6.Click += new System.EventHandler(this.toolStripMenuItem6_Click);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem7});
            this.contextMenuStrip3.Name = "contextMenuStrip3";
            this.contextMenuStrip3.Size = new System.Drawing.Size(101, 26);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem7.Text = "清屏";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // contextMenuStrip4
            // 
            this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem8});
            this.contextMenuStrip4.Name = "contextMenuStrip4";
            this.contextMenuStrip4.Size = new System.Drawing.Size(101, 26);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(100, 22);
            this.toolStripMenuItem8.Text = "复制";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // toolStripMenuItem9
            // 
            this.toolStripMenuItem9.Name = "toolStripMenuItem9";
            this.toolStripMenuItem9.Size = new System.Drawing.Size(124, 22);
            this.toolStripMenuItem9.Text = "复制";
            this.toolStripMenuItem9.Click += new EventHandler(toolStripMenuItem9_Click);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.contextMenuStrip3.ResumeLayout(false);
            this.contextMenuStrip4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        void toolStripMenuItem9_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.SelectedText);
        }

        void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(this.SelectedText);
        }

        void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            this.Clear();   
        }

        void toolStripMenuItem6_Click(object sender, EventArgs e)
        {
            this.Clear();            
        }

        void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {              
                ImageForm form = new ImageForm(this.imageOnRightClick);
                form.Show();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }            
        }

        //保存图片
        void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            try
            {
                
                bool gif = ImageHelper.IsGif(this.imageOnRightClick);
                string postfix = gif ? "gif" : "jpg";

                string path = this.GetPathToSave("请选择保存路径", "image." + postfix, null);
                if (path == null)
                {
                    return;
                }
                ImageFormat format = gif ? ImageFormat.Gif : ImageFormat.Jpeg;

                ImageHelper.Save(this.imageOnRightClick, path, format);
                MessageBox.Show("成功保存图片。");
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        //复制
        void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.SetImage(this.imageOnRightClick);               
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            try
            {
                if (Clipboard.ContainsImage())
                {
                    Image img = Clipboard.GetImage();
                    if (img != null)
                    {
                        this.InsertImage(img);
                    }
                    return;
                }

                this.Paste();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            try
            {
                string file = this.GetFileToOpen2("请选择图片", null, ".jpg", ".bmp", ".png", ".gif");
                if (file == null)
                {
                    return;
                }

                Image img = Image.FromStream(new MemoryStream(File.ReadAllBytes(file)));
                this.InsertImage(img);
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.Message);
            }
        }

        private string GetFileToOpen2(string title, string iniDir, params string[] extendNames)
        {
            StringBuilder filterBuilder = new StringBuilder("(");
            for (int i = 0; i < extendNames.Length; i++)
            {
                filterBuilder.Append("*");
                filterBuilder.Append(extendNames[i]);
                if (i < extendNames.Length - 1)
                {
                    filterBuilder.Append(";");
                }
                else
                {
                    filterBuilder.Append(")");
                }
            }
            filterBuilder.Append("|");
            for (int i = 0; i < extendNames.Length; i++)
            {
                filterBuilder.Append("*");
                filterBuilder.Append(extendNames[i]);
                if (i < extendNames.Length - 1)
                {
                    filterBuilder.Append(";");
                }
            }

            OpenFileDialog openDlg = new OpenFileDialog();
            openDlg.Filter = filterBuilder.ToString();
            openDlg.FileName = "";
            openDlg.InitialDirectory = iniDir;
            if (title != null)
            {
                openDlg.Title = title;
            }

            openDlg.CheckFileExists = true;
            openDlg.CheckPathExists = true;

            DialogResult res = openDlg.ShowDialog();
            if (res == DialogResult.OK)
            {
                return openDlg.FileName;
            }

            return null;
        } 
        #endregion
    }
}
