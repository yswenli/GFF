/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：14a358c4-cc27-4ff5-a85f-081fba7b2bdc
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：Em.MCIFrameWork.Remote
 * 类名称：RemoteScreenUserControl
 * 文件名：RemoteScreenUserControl
 * 创建年份：2015
 * 创建时间：2015-12-03 14:54:20
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GFF.Helper.Extention;
using GFF.Model.Enum;
using GFF.MS;
using MouseAndKeyHelper = GFF.Helper.Win32.MouseAndKeyHelper;

namespace GFF.Component.Remote
{
    /// <summary>
    /// 服务器接收到客户端远程协助请求控件
    /// 启动远程服务器功能（1.绘制屏幕2.收集并发送鼠标键盘命令）
    /// </summary>
    public partial class RemoteScreenUserControl : UserControl
    {
        MessageClient _mClient = null;


        public delegate void OnAcceptedHandler();
        /// <summary>
        /// 服务器接收到客户端时
        /// </summary>
        public OnAcceptedHandler OnAccepted;
        /// <summary>
        /// 服务器接收到客户端时
        /// </summary>
        /// <param name="nHelper"></param>
        protected void RaiseOnAccepted()
        {
            OnAccepted?.Invoke();
        }


        public RemoteScreenUserControl()
        {
            InitializeComponent();
        }


        MouseAndKeyHelper _MouseAndKeyHelper = new MouseAndKeyHelper();

        /// <summary>
        /// 是否接收到helpcommand
        /// </summary>
        public bool IsHelpered
        {
            get; private set;
        }

        string _remote = string.Empty;

        /// <summary>
        /// 启动远程服务器功能（1.绘制屏幕2.收集并发送鼠标键盘命令）
        /// </summary>
        /// <param name="userName"></param>
        public void Init(string userName)
        {
            _mClient = new MessageClient("wenlirdp_" + userName);
            _mClient.OnMessage += _mClient_OnMessage;
            _mClient.OnFile += _mClient_OnFile;
            _mClient.ConnectAsync();
        }

        private void _mClient_OnMessage(object sender, Model.Entity.Message msg)
        {
            var transfer = Encoding.UTF8.GetString(msg.Data);

            if (transfer == "wenlirdp")
            {
                _remote = msg.Sender;
                RaiseOnAccepted();
            }
            _mClient.SendPrivateMsg(_remote, "wenlirdp");



        }

        /// <summary>
        /// 接收远程桌面的图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        private void _mClient_OnFile(object sender, Model.Entity.Message msg)
        {
            if (msg != null && msg.Data != null)
            {
                var datas = msg.Data;
                Task.Factory.StartNew(() =>
                {
                    this.InvokeAction(() =>
                    {
                        try
                        {
                            MemoryStream ms = new MemoryStream(datas);
                            var bitmap = new Bitmap(ms);
                            Graphics g = this.CreateGraphics();
                            Bitmap canvas = new Bitmap(this.Width, this.Height = this.Height == 0 ? 1 : this.Height);//创建一块画布
                            Graphics graphics = Graphics.FromImage(canvas);
                            graphics.DrawImage(bitmap, 0, 0, this.Width, this.Height);//将图像绘制在画布上
                            graphics.DrawString("图片大小" + datas.Length + ";像素：w" + bitmap.Width + "h" + bitmap.Height, new Font("微软雅黑", 13), Brushes.Red, new PointF(100, 100));
                            graphics.Flush();
                            g.DrawImage(canvas, 0, 0, this.Width, this.Height);//将画布绘制在Panel上
                            canvas.Dispose();
                            graphics.Dispose();
                            g.Dispose();
                            bitmap.Dispose();
                            ms.Dispose();
                        }
                        catch { }

                    });
                });
                msg.Data = null;
                msg = null;
            }
        }

        /// <summary>
        /// 服务器接收到消息错误时
        /// </summary>
        /// <param name="ex"></param>
        void transferHelper_OnServerReceivErred(Exception ex)
        {
            this.RaiseOnOffLined("服务器接收到消息异常：" + ex.Message);
            this.Stop();
        }


        private void FormRemoteHelpDesktop_MouseWheel(object sender, MouseEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    _mClient.SendPrivateMsg(_remote, "mousewheel:" + e.Delta.ToString());
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_MouseDown(object sender, MouseEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    var transferStr = "mousedown";
                    if (e.Button == MouseButtons.Left)
                        transferStr += ":left";
                    else if (e.Button == MouseButtons.Middle)
                        transferStr += ":middle";
                    else if (e.Button == MouseButtons.Right)
                        transferStr += ":right";
                    _mClient.SendPrivateMsg(_remote, transferStr);
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    var transferStr = "mousemove:" + e.X + "@" + e.Y + "@" + this.Height;
                    _mClient.SendPrivateMsg(_remote, transferStr);
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_MouseUp(object sender, MouseEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    var transferStr = "mouseup:";
                    if (e.Button == MouseButtons.Left)
                        transferStr += "left";
                    else if (e.Button == MouseButtons.Middle)
                        transferStr += "middle";
                    else if (e.Button == MouseButtons.Right)
                        transferStr += "right";
                    _mClient.SendPrivateMsg(_remote, transferStr);
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    if (e.KeyCode == Keys.Escape)
                    {
                        if (MessageBox.Show("需要关闭远程协助吗？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            var transferStr = "StopRemoteHelp:";
                            _mClient.SendPrivateMsg(_remote, transferStr);
                            this.RaiseOnEscUp();
                        }
                    }
                    else if (e.Control && e.KeyCode == Keys.Enter)
                    {
                        if (this.Height <= 450)
                        {
                            this.Height = Screen.AllScreens[0].Bounds.Height;
                            this.Location = new Point(0, 0);
                            var transferStr = "FullScreen:";
                            _mClient.SendPrivateMsg(_remote, transferStr);
                        }
                        else
                        {
                            this.Height = 450;
                            var transferStr = "PartScreen:";
                            _mClient.SendPrivateMsg(_remote, transferStr);
                        }
                    }
                }
                catch
                {

                }
            }
        }

        private void RemoteScreenUserControl_KeyUp(object sender, KeyEventArgs e)
        {
            if (this._mClient != null)
            {
                try
                {
                    var transferStr = "KeyPress:" + e.KeyValue.ToString();
                    _mClient.SendPrivateMsg(_remote, transferStr);
                }
                catch
                {

                }
            }
        }

        public delegate void OnEscUpHandler();
        /// <summary>
        /// 协助方按ESC键结束协助
        /// </summary>
        public event OnEscUpHandler OnEscUp;

        protected void RaiseOnEscUp()
        {
            if (this.OnEscUp != null)
            {
                _mClient.Dispose();
                _mClient = null;
                this.OnEscUp();
            }
        }


        public delegate void OnOffLinedHandler(string msg);
        /// <summary>
        /// 接受远程桌面图片对方掉线时事件
        /// </summary>
        public event OnOffLinedHandler OnOffLined;

        protected void RaiseOnOffLined(string msg)
        {
            if (this.OnOffLined != null)
            {
                this.OnOffLined(msg);
            }
        }

        /// <summary>
        /// 关闭远程协助
        /// </summary>
        public void Stop()
        {
            try
            {
                _mClient.Dispose();
                _mClient = null;
            }
            catch { }

        }
    }
}
