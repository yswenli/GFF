/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2015-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.17929
 * 唯一标识：85862f4b-d65c-46af-857e-a93e95df2ab5
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：Em.MCIFrameWork.Remote
 * 类名称：RemoteHelper
 * 文件名：RemoteHelper
 * 创建年份：2015
 * 创建时间：2015-11-24 16:35:09
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Threading.Tasks;
using GFF.Helper;
using GFF.Helper.Win32;
using GFF.Model.Enum;
using System.Text;
using GFF;
using GFF.MS;

namespace GFF.Component.Remote
{
    /// <summary>
    /// 请求方桌面共享、远程协助工具类
    /// </summary>
    public class RequestRemoteHelper : IDisposable
    {
        MessageClient _mCleint = null;

        private bool _isStartCapture;

        private bool _isHelp;

        QualityLevelEnum _qualityLevel;

        int _fps = 3;

        private MouseAndKeyHelper _MouseAndKeyHelper = null;

        private bool _isClose = false;

        string _remote = string.Empty;

        public event Action OnConnected;

        /// <summary>
        /// 桌面共享、远程协助工具类
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="remote"></param>
        public RequestRemoteHelper(string userName)
        {
            _MouseAndKeyHelper = new MouseAndKeyHelper();

            _mCleint = new MessageClient("wenlirdp_" + userName);

            _mCleint.OnMessage += webCient_OnMessage;
        }

        private void SendRequest()
        {
            _mCleint.SendPrivateMsg(_remote, "wenlirdp");
        }

        /// <summary>
        /// 发送图像到远程
        /// </summary>
        private void SendImage()
        {
            Task.Factory.StartNew(() =>
            {
                Rectangle rectangle = new Rectangle(0, 0, Screen.AllScreens[0].Bounds.Width, Screen.AllScreens[0].Bounds.Height);
                while (!_isClose)
                {
                    if (_isStartCapture)
                    {
                        using (Bitmap desktopImage = new Bitmap(rectangle.Width, rectangle.Height))
                        {
                            using (Graphics g = Graphics.FromImage(desktopImage))
                            {
                                g.CopyFromScreen(0, 0, 0, 0, desktopImage.Size);
                                MouseAndKeyHelper.DrawMouse(g);
                                using (MemoryStream ms = ImageHelper.GetLossyCompression(desktopImage, (int)_qualityLevel, "W", 800))
                                {
                                    if (desktopImage != null)
                                    {
                                        try
                                        {
                                            ms.Position = 0;
                                            _mCleint.SendFileAsync(_remote, ms.GetBuffer());
                                            var sec = 1000;
                                            if (_fps > 0)
                                                sec = 1000 / _fps;
                                            else
                                                sec = 2000;
                                            Thread.Sleep(sec);
                                        }
                                        catch
                                        {
                                        }
                                        finally
                                        {
                                            ms.Dispose();
                                            g.Dispose();
                                            desktopImage.Dispose();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            });
        }

        /// <summary>
        /// 接收到命令
        /// </summary>
        /// <param name="msg"></param>
        private void webCient_OnMessage(object sender, GFF.Model.Entity.Message msg)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var transferStr = Encoding.UTF8.GetString(msg.Data).ToLower();

                    if (transferStr == "wenlirdp")
                    {
                        OnConnected?.Invoke();
                        SendImage();
                    }
                    else
                    {
                        if (this._isHelp)
                        {
                            var transferArr = transferStr.Split(new string[] { ":" }, StringSplitOptions.None);
                            var command = transferArr[0];
                            var dataStr = transferArr[1];
                            switch (command)
                            {
                                case "mousemove"://鼠标移动
                                    string[] localString = dataStr.Split('@');
                                    int localX = int.Parse(localString[0]);
                                    int localY = int.Parse(localString[1]);
                                    int remoteFormHeight = int.Parse(localString[2]);
                                    double rate = ((double)Screen.AllScreens[0].Bounds.Height) / remoteFormHeight;
                                    _MouseAndKeyHelper.SetCursorPosition((int)(localX * rate), (int)(localY * rate));
                                    break;
                                case "mousedown"://鼠标键的按下
                                    if (dataStr == "left")
                                        _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.LeftMouse);
                                    else if (dataStr == "middle")
                                        _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.MiddleMouse);
                                    else if (dataStr == "right")
                                        _MouseAndKeyHelper.MouseDown(MouseAndKeyHelper.ClickOnWhat.RightMouse);
                                    break;
                                case "mouseup"://鼠标键的抬起
                                    if (dataStr == "left")
                                        _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.LeftMouse);
                                    else if (dataStr == "middle")
                                        _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.MiddleMouse);
                                    else if (dataStr == "right")
                                        _MouseAndKeyHelper.MouseUp(MouseAndKeyHelper.ClickOnWhat.RightMouse);
                                    break;
                                case "keypress"://键盘键的按下即抬起
                                    int keyValue = int.Parse(dataStr);
                                    MouseAndKeyHelper.VirtualKeys virtualKey = (MouseAndKeyHelper.VirtualKeys)(Enum.ToObject(typeof(MouseAndKeyHelper.VirtualKeys), keyValue));
                                    _MouseAndKeyHelper.KeyPress(virtualKey);
                                    break;
                                case "mousewheel"://鼠标滚轮的滚动
                                    int delta = int.Parse(dataStr);
                                    _MouseAndKeyHelper.MouseWheel(delta);
                                    break;
                                default:
                                    break;
                            }

                        }
                        if (transferStr == "helpcommand")
                        {
                            this._isHelp = true;
                        }
                        if (transferStr == "stopremotehelp")
                        {
                            this._isHelp = false;
                            _mCleint.Dispose();
                        }
                    }


                }
                catch { }
            });
        }

        #region 请求方处理        

        Thread captureThread;
        /// <summary>
        /// 开始桌面共享、远程协助（发送图片）
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="isHelp"></param>
        /// <param name="qualityLevel"></param>
        /// <param name="fps"></param>
        public void StartSendCapture(string remote, bool isHelp = true, QualityLevelEnum qualityLevel = QualityLevelEnum.Normal, int fps = 1)
        {
            _isHelp = isHelp;
            _qualityLevel = qualityLevel;
            _fps = fps;
            _isStartCapture = true;
            _mCleint.ConnectAsync();
            _remote = "wenlirdp_" + remote;
            while (!_mCleint.IsConnected)
            {
                Thread.Sleep(10);
            }
            this.SendRequest();
        }


        public delegate void OnOffLinedHandler(string msg);
        /// <summary>
        /// 协助方无法收取屏幕
        /// </summary>
        public event OnOffLinedHandler OnOffLined;
        /// <summary>
        /// 协助方无法收取屏幕
        /// </summary>
        /// <param name="msg"></param>
        protected void RaiseOnOffLined(string msg)
        {
            if (this.OnOffLined != null)
            {
                this.OnOffLined(msg);
            }
        }
        /// <summary>
        /// 关闭桌面共享
        /// </summary>
        public void StopSendCapture()
        {
            _isStartCapture = false;
        }

        #endregion

        public void Dispose()
        {

            this._isClose = true;
            _isStartCapture = false;
            _mCleint.Dispose();
        }
    }


    public enum QualityLevelEnum
    {
        High = 95,
        Normal = 70,
        Low = 50
    }


}
