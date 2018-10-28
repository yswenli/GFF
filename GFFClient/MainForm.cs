/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：20da4241-0bdc-4a06-8793-6d0889c31f95
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：MCITest


 * 创建年份：2015
 * 创建时间：2015-12-02 11:15:24
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using CCWin;
using CCWin.SkinClass;
using CCWin.SkinControl;
using GFF.Component.Capture;
using GFF.Component.Config;
using GFF.Component.Emotion;
using GFF.Helper;
using GFF.Helper.Extention;
using GFFClient.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ImageHelper = GFF.Helper.ImageHelper;

namespace GFFClient
{
    public partial class MainForm : CCSkinMain
    {
        private Image fileImage = Resources.file;

        public static PushHelper PushHelper = new PushHelper();

        #region 无参构造

        public MainForm()
        {
            InitializeComponent();
            InitEmotion();
        }

        #endregion

        #region 初始化表情

        /// <summary>
        ///     初始化表情
        /// </summary>
        private void InitEmotion()
        {
            GlobalResourceManager.Initialize();
            chatBoxSend.Initialize(GlobalResourceManager.EmotionDictionary);
            chatBox_history.Initialize(GlobalResourceManager.EmotionDictionary);
        }

        #endregion

        #region 窗体重绘时

        private void FrmQQChat_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.None;
            //全屏蒙浓遮罩层
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, 255, 255, 255)),
                new Rectangle(0, 0, Width, chatBox_history.Top));
            g.FillRectangle(new SolidBrush(Color.FromArgb(80, 255, 255, 255)),
                new Rectangle(0, chatBox_history.Top, chatBox_history.Width + chatBox_history.Left,
                    Height - chatBox_history.Top));
            //线条
            g.DrawLine(new Pen(Color.FromArgb(180, 198, 221)), new Point(0, chatBox_history.Top - 1),
                new Point(chatBox_history.Right, chatBox_history.Top - 1));
            g.DrawLine(new Pen(Color.FromArgb(180, 198, 221)), new Point(0, chatBox_history.Bottom),
                new Point(chatBox_history.Right, chatBox_history.Bottom));
        }

        #endregion

        #region 发送信息

        private void btnSend_Click(object sender, EventArgs e)
        {
            var content = chatBoxSend.GetContent();
            //发送内容为空时，不做响应
            if (content.IsEmpty())
                return;

            var url = string.Empty;
            if (content.ContainsForeignImage())
            {
                foreach (var item in content.ForeignImageDictionary)
                    try
                    {
                        url += string.Format("[img={0}]{1}", ClientConfig.Instance().Url + ImageHelper.ToUrl(ClientConfig.Instance().Url + "Upload", item.Value),
                            Environment.NewLine);
                    }
                    catch
                    {
                    }
                content.ForeignImageDictionary.Clear();
                content.Text = url;
            }
            var msgtxt = content.Text;

            //清空发送输入框           
            chatBoxSend.Text = string.Empty;
            chatBoxSend.Focus();

            if (string.IsNullOrEmpty(msgtxt) && (content.EmotionDictionary.Count == 0))
                return;

            GFF.Component.ChatBox.ChatBoxContent c = new GFF.Component.ChatBox.ChatBoxContent();
            c.Text = msgtxt;
            c.EmotionDictionary = content.EmotionDictionary;
            c.PicturePositions = content.PicturePositions;
            c.Font = content.Font;
            c.Color = content.Color;
            string msg = SerializeHelper.Serialize(c);
            ThreadPool.QueueUserWorkItem(s => SendMsgToServer(msg));            
        }

        #endregion

        #region 退出当前聊天

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region 发送文件

        private void ToolFile_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "请选择文件";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "文件|*.*";
            openFileDialog1.Multiselect = false;
            openFileDialog1.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
                PushHelper.HttpSendFileAsync(openFileDialog1.FileName, url =>
                {
                    try
                    {
                        var fileUrl = string.Format("[file={0}]{1}", url, Environment.NewLine);
                        var content = new GFF.Component.ChatBox.ChatBoxContent();
                        content.Text = fileUrl;
                        SendMsgToServer(SerializeHelper.Serialize(content));
                    }
                    catch
                    {
                    }
                });
        }

        #endregion

        #region 点击表情按钮事件

        private void toolCountenance_Click(object sender, EventArgs e)
        {
            var pt = PointToScreen(new Point(skToolMenu.Left + 30 - FaceForm.Width / 2, skToolMenu.Top - FaceForm.Height));
            FaceForm.Show(pt.X, pt.Y, skToolMenu.Height);
        }

        #endregion

        #region 震动按钮事件

        private void toolZhenDong_Click(object sender, EventArgs e)
        {
            var msg = "发送了一个抖动提醒。\n";
            //this.AppendMessage(this.QQUser.DisplayName, Color.Green, msg);
            chatBoxSend.Focus();
            VibrationHelper.Vibration(this);
            var content = new GFF.Component.ChatBox.ChatBoxContent();
            content.Text = msg;
            SendMsgToServer(SerializeHelper.Serialize(content));
        }

        #endregion

        #region 字体

        //显示字体对话框
        private void toolFont_Click(object sender, EventArgs e)
        {
            fontDialog.Font = chatBoxSend.Font;
            fontDialog.Color = chatBoxSend.ForeColor;
            if (DialogResult.OK == fontDialog.ShowDialog())
            {
                chatBoxSend.Font = fontDialog.Font;
                chatBoxSend.ForeColor = fontDialog.Color;
            }
        }

        #endregion

        #region 自定义系统按钮事件

        private void FrmQQChat_SysBottomClick(object sender, SysButtonEventArgs e)
        {
            if (e.SysButton.Name == "SysSet")
            {
                var l = PointToScreen(e.SysButton.Location);
                l.Y += e.SysButton.Size.Height + 1;
                SysMenu.Show(l);
            }
        }

        #endregion

        #region 远程

        private void toolStripButton5_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region 退出

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            PushHelper.Stop();
            try
            {
                Environment.Exit(-1);
            }
            catch
            {
            }
        }

        #endregion

        #region 变量

        /// <summary>
        ///     文本格式
        /// </summary>
        private readonly Font messageFont = new Font("微软雅黑", 9);

        private ChatListSubItem qqUser;

        /// <summary>
        ///     QQ聊天用户
        /// </summary>
        public ChatListSubItem QQUser
        {
            get
            {
                return qqUser;
            }
            set
            {
                if (qqUser != value)
                {
                    qqUser = value;
                    lblChatName.Tag =
                        lblChatName.Text =
                            string.IsNullOrEmpty(qqUser.DisplayName) ? qqUser.NicName : qqUser.DisplayName;
                    lblChatQm.Text = qqUser.PersonalMsg;
                    pnlImgTx.BackgroundImage = qqUser.HeadImage;
                    //imgQQShow.Image = qqUser.QQShow;
                }
            }
        }

        #endregion

        #region 带参构造

        public MainForm(ChatListSubItem QQUser)
            : this()
        {
            this.QQUser = QQUser;

            chatBox_history.ContextMenuMode = GFF.Component.ChatBox.ChatBoxContextMenuMode.ForOutput;

            PushHelper.OnMessage += pushHelper_OnMessage;

            PushHelper.Start(this.QQUser.DisplayName, "all");

            //
            chatListBox.DoubleClickSubItem += ChatListBox_DoubleClickSubItem;
            FriendHelper.OnListChanged += FriendHelper_OnListChanged;
        }

        private void ChatListBox_DoubleClickSubItem(object sender, ChatListEventArgs e, MouseEventArgs es)
        {
            if (e.SelectSubItem != null)
                chatBoxSend.Text += ("@" + e.SelectSubItem.DisplayName + " ");
        }

        private void FriendHelper_OnListChanged(List<Friend> list)
        {
            BeginInvoke(new Action(() =>
            {
                if ((list != null) && (list.Count > 0))
                {
                    chatListBox.Items[0].SubItems.Clear();
                    list.ForEach(item =>
                    {
                        var s = new ChatListSubItem
                        {
                            DisplayName = item.UserName
                        };
                        chatListBox.Items[0].SubItems.Add(s);
                    });
                }
            }), null);
        }

        #endregion

        #region 接收信息封装

        private void OnReceivedMsg(GFF.Component.ChatBox.ChatBoxContent content, DateTime? originTime)
        {
            AppendChatBoxContent(lblChatName.Tag == null ? "斯" : lblChatName.Tag.ToString(), originTime, content,
                Color.Blue, false);
        }

        private void OnReceivedMsg(string msg)
        {
            try
            {
                var userName = string.Empty;
                var json = string.Empty;
                var content = new GFF.Component.ChatBox.ChatBoxContent();
                if (!string.IsNullOrEmpty(msg) && (msg.IndexOf("|") > -1))
                {
                    userName = msg.Substring(0, msg.IndexOf("|"));
                    json = msg.Substring(msg.IndexOf("|") + 1);
                    try
                    {
                        content = SerializeHelper.Deserialize<GFF.Component.ChatBox.ChatBoxContent>(json);
                    }
                    catch
                    {
                        return;
                    }
                    content = SerializeHelper.Deserialize<GFF.Component.ChatBox.ChatBoxContent>(json);
                    FriendHelper.Set(userName);
                }
                if (userName != qqUser.DisplayName)
                    if (content.Text == "发送了一个抖动提醒。\n")
                    {
                        chatBoxSend.Focus();
                        VibrationHelper.Vibration(this);
                    }
                if (content.Text.IndexOf("[img=") > -1)
                {
                    var fileUrls = content.Text.Split(new[] { @"\r\n" }, StringSplitOptions.None);
                    foreach (var item in fileUrls)
                    {
                        var imageUrl = item.Substring(item.IndexOf("[img=") + 5);
                        imageUrl = imageUrl.Substring(0, imageUrl.IndexOf("]"));
                        var img = ImageHelper.FromUrl(imageUrl);
                        content.AddForeignImage(0, img);
                    }
                    content.Text = " ";
                }
                if (content.Text.IndexOf("[file=") > -1)
                {
                    var fileUrls = content.Text.Split(new[] { @"\r\n" }, StringSplitOptions.None);
                    content.Text = "";
                    foreach (var item in fileUrls)
                    {
                        var fileUrl = item.Substring(item.IndexOf("[file=") + 6);
                        fileUrl = fileUrl.Substring(0, fileUrl.IndexOf("]"));
                        content.Text += fileUrl + Environment.NewLine;
                    }
                }
                AppendChatBoxContent(userName, null, content, Color.Blue, false);
            }
            catch
            {
            }
        }

        #endregion

        #region 发送消息封装

        /// <summary>
        ///     发送信息文本到内容框
        /// </summary>
        /// <param name="userName">名字</param>
        /// <param name="originTime">时间</param>
        /// <param name="content">发送内容</param>
        /// <param name="color">字体颜色</param>
        /// <param name="followingWords">是否有用户名</param>
        private void AppendChatBoxContent(string userName, DateTime? originTime, GFF.Component.ChatBox.ChatBoxContent content, Color color,
            bool followingWords)
        {
            AppendChatBoxContent(userName, originTime, content, color, followingWords, originTime != null);
        }

        /// <summary>
        ///     发送信息文本到内容框
        /// </summary>
        /// <param name="userName">名字</param>
        /// <param name="originTime">时间</param>
        /// <param name="content">发送内容</param>
        /// <param name="color">字体颜色</param>
        /// <param name="followingWords">是否有用户名</param>
        /// <param name="offlineMessage">是否在线消息</param>
        private void AppendChatBoxContent(string userName, DateTime? originTime, GFF.Component.ChatBox.ChatBoxContent content, Color color,
            bool followingWords, bool offlineMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke(
                    new Action(
                        () =>
                        {
                            AppendChatBoxContent(userName, originTime, content, color, followingWords, offlineMessage);
                        }));
            }
            else
            {
                if (!followingWords)
                {
                    var showTime = DateTime.Now.ToLongTimeString();
                    if (!offlineMessage && (originTime != null))
                        showTime = originTime.Value.ToString();
                    if (userName == QQUser.DisplayName)
                        color = Color.YellowGreen;
                    chatBox_history.AppendRichText(string.Format("{0}  {1}\n", userName, showTime),
                        new Font(messageFont, FontStyle.Regular), color);
                    if ((originTime != null) && offlineMessage)
                        chatBox_history.AppendText(string.Format("    [{0} 离线消息] ", originTime.Value));
                    else
                        chatBox_history.AppendText("    ");
                }
                else
                {
                    chatBox_history.AppendText("   .");
                }
                chatBox_history.AppendChatBoxContent(content);
                chatBox_history.AppendText("  \n");
                chatBox_history.Focus();
                chatBox_history.Select(chatBox_history.Text.Length, 0);
                chatBox_history.ScrollToCaret();
                chatBoxSend.Focus();
            }
        }

        /// <summary>
        ///     发送信息文本到内容框
        /// </summary>
        /// <param name="userName">名称</param>
        /// <param name="color">字体颜色</param>
        /// <param name="msg">信息</param>
        private void AppendMessage(string userName, Color color, string msg)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() =>
                {
                    AppendMessage(userName, color, msg);
                }));
            }
            else
            {
                var showTime = DateTime.Now;
                chatBox_history.AppendRichText(string.Format("{0}  {1}\n", userName, showTime.ToLongTimeString()),
                    new Font(messageFont, FontStyle.Regular), color);
                chatBox_history.AppendText("    ");

                chatBox_history.AppendText(msg);
                chatBox_history.Select(chatBox_history.Text.Length, 0);
                chatBox_history.ScrollToCaret();
            }
        }

        /// <summary>
        ///     发送系统消息
        /// </summary>
        /// <param name="msg">信息</param>
        public void AppendSysMessage(string msg)
        {
            AppendMessage("系统", Color.Gray, msg);
            chatBox_history.AppendText("\n");
        }

        #endregion

        #region 服务器转发

        private void pushHelper_OnMessage(string channelID, string msg)
        {
            if (channelID == "count")
                Invoke(new MethodInvoker(delegate
                {
                    skinLabel1.Text = "当前在线人数：" + msg + "人";
                }));
            else
                Invoke(new MethodInvoker(delegate
                {
                    //OnReceivedMsg(new Em.MCIFrameWork.Controls.ChatBox.ChatBoxContent(msg, messageFont, Color.Black), null);

                    OnReceivedMsg(msg);
                }));
        }


        Regex reg = new Regex("@.*\",\"Font");
        /// <summary>
        ///     发送消息到服务器
        /// </summary>
        /// <param name="msg"></param>
        private void SendMsgToServer(string msg)
        {
            var s = string.Empty;
            try
            {
                //私信                
                if (reg.IsMatch(msg))
                {
                    var receiver = msg.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries)[3];
                    receiver = receiver.Substring(1);
                    receiver = receiver.Split(new string[] { " ", ":", "：", "　" }, StringSplitOptions.RemoveEmptyEntries)[0];
                    PushHelper.Client.SendPrivateMsg(receiver, qqUser.DisplayName + "|" + msg);
                    AppendChatBoxContent(qqUser.DisplayName, null, SerializeHelper.Deserialize<GFF.Component.ChatBox.ChatBoxContent>(msg),
                        Color.YellowGreen, false);
                }
                else
                {
                    PushHelper.Publish("all", qqUser.DisplayName + "|" + msg);
                }

                OnReceivedMsg(qqUser.DisplayName + "|" + msg);
            }
            catch (Exception)
            {
                s = "Server挂了！现在时间是：" + DateTime.Now;
            }
        }

        #endregion

        #region 发图

        private void toolStripButton1_ButtonClick(object sender, EventArgs e)
        {
            ToolStripMenuItem1_Click(sender, e);
        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "请选择图片";
            openFileDialog1.CheckFileExists = true;
            openFileDialog1.CheckPathExists = true;
            openFileDialog1.Filter = "图片|*.jpg;*.png;*.bmp;*.gif";
            openFileDialog1.Multiselect = false;
            openFileDialog1.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
            openFileDialog1.FileName = string.Empty;
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                loadingTxt.Show();
                chatBoxSend.Enabled = false;
                this.InvokeAction(() =>
                {
                    var img = ImageHelper.MakeThumbnail(Image.FromFile(openFileDialog1.FileName), 200, 100);
                    chatBoxSend.InsertImage(img);
                    loadingTxt.Hide();
                    chatBoxSend.Enabled = true;
                    chatBoxSend.Focus();
                    chatBoxSend.ScrollToCaret();
                });
            }
        }

        #endregion

        #region 截图

        /// <summary>
        ///     截图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPrintScreen_ButtonClick(object sender, EventArgs e)
        {
            StartCapture();
        }

        private CaptureForm frmCapture;
        //截图方法
        private void StartCapture()
        {
            if ((frmCapture == null) || frmCapture.IsDisposed)
            {
                frmCapture = new CaptureForm();
                frmCapture.OnCaptured += frmCapture_OnCaptured;
            }
            frmCapture.IsCaptureCursor = true;
            frmCapture.IsFromClipBoard = false;
            frmCapture.Show();
        }

        private void frmCapture_OnCaptured(Image imgDatas)
        {
            chatBoxSend.InsertImage(imgDatas);
            chatBoxSend.Focus();
            chatBoxSend.ScrollToCaret();
        }

        #endregion

        #region 表情窗体与事件（初始化表情）

        public EmotionForm _faceForm;

        public EmotionForm FaceForm
        {
            get
            {
                if (_faceForm == null)
                {
                    _faceForm = new EmotionForm
                    {
                        ImagePath = "Emotion\\",
                        CustomImagePath = "Emotion\\Custom\\",
                        CanManage = true,
                        ShowDemo = true
                    };
                    _faceForm.Init(24, 24, 8, 8, 12, 8);
                    _faceForm.Selected += _faceForm_Selected;
                    _faceForm.OnChanged += _faceForm_OnChanged;
                }

                return _faceForm;
            }
        }

        private void _faceForm_Selected(object sender, SelectFaceArgs e)
        {
            chatBoxSend.InsertDefaultEmotion((uint)e.ImageIndex);
        }

        private void _faceForm_OnChanged()
        {
            InitEmotion();
        }

        #endregion

        #region 发送键更多选择 及 发送键更多选择菜单关闭时

        //发送键更多选择
        private void btnSendMenu_Click(object sender, EventArgs e)
        {
            btnSendMenu.StopState = StopStates.Pressed;
            SendMenu.Show(btnSendMenu, new Point(0, btnSendMenu.Height + 5));
        }

        //发送键更多选择菜单关闭时
        private void SendMenu_Closing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            btnSendMenu.StopState = StopStates.NoStop;
            btnSendMenu.ControlState = ControlState.Normal;
        }

        #endregion
    }
}