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

using System;
using System.Text;
using System.Windows.Forms;
using GFF;
using GFF.Component.Remote;
using GFF.Helper;
using GFF.Helper.Extention;
using GFF.Model.Enum;
using CCWin;

namespace GFFClient
{
    public partial class RemoteHelpForm : CCSkinMain
    {
        private int _fps = 10;

        private bool _isHelp;

        private QualityLevelEnum _qualityLevel = QualityLevelEnum.Normal;

        private string _serverIP;

        private int _serverPort;

        private string _userName;

        private string _password;

        private string ip;

        private int port;

        /// <summary>
        ///     请求方
        /// </summary>
        private RequestRemoteHelper requestRemoteHelper1;

        public RemoteHelpForm(string userName, string password)
        {
            InitializeComponent();

            this._serverIP = ClientConfig.Instance().IP;
            this._serverPort = ClientConfig.Instance().UdpPort;
            this._userName = userName;
            this._password = password;

            skinProgressBar1.Visible = false;
            comboBox1.SelectedIndex = 1;
            label2.Text = _fps + " fps";
            label3.Text = "";

            remoteScreenUserControl1.OnAccepted += RemoteScreenUserControl_OnAccepted;
            remoteScreenUserControl1.OnOffLined += remoteScreenUserControl1_OnOffLined;
            remoteScreenUserControl1.OnEscUp += remoteScreenUserControl1_OnEscUp;
            remoteScreenUserControl1.Init(this._serverIP, this._serverPort, this._userName, this._password);
        }

        private void RemoteHelpForm_Load(object sender, EventArgs e)
        {
            //

            label1.Text = "正在等待远程协助";
            //
            SizedForm(false);
            //
            requestRemoteHelper1 = new RequestRemoteHelper(this._serverIP, this._serverPort, this._userName, this._password);
            requestRemoteHelper1.OnOffLined += remoteServerHelper1_OnOffLined;
        }

        /// <summary>
        ///     响应外部连接
        /// </summary>
        private void RemoteScreenUserControl_OnAccepted()
        {
            label1.InvokeAction(() =>
            {
                label1.Text = "正在响应远程协助...";
                textBox1.Enabled =
                    checkBox1.Enabled = comboBox1.Enabled = skinTrackBar1.Enabled = button1.Enabled = false;
                skinProgressBar1.Visible = true;
                SizedForm(true);
            });
        }

        /// <summary>
        ///     网络异常时
        /// </summary>
        /// <param name="msg"></param>
        private void remoteScreenUserControl1_OnOffLined(string msg)
        {
            label1.InvokeAction(() =>
            {
                label1.Text = "正在等待远程协助";
                textBox1.Enabled =
                    checkBox1.Enabled = comboBox1.Enabled = skinTrackBar1.Enabled = button1.Enabled = true;
                skinProgressBar1.Visible = false;
                SizedForm(false);
                remoteScreenUserControl1.Stop();
                remoteScreenUserControl1.Init(this._serverIP, this._serverPort, this._userName, this._password);
            });
        }

        /// <summary>
        ///     按ESC键
        /// </summary>
        private void remoteScreenUserControl1_OnEscUp()
        {
            this.Close();
        }

        /// <summary>
        ///     点击按钮请求连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = checkBox1.Enabled = comboBox1.Enabled = skinTrackBar1.Enabled = button1.Enabled = false;

            skinProgressBar1.Visible = true;
            try
            {
                requestRemoteHelper1.StartSendCapture(textBox1.Text, _isHelp, _qualityLevel, _fps);
                label1.Text = textBox1.Text + "对方已响应了你的远程协助请求";
                skinProgressBar1.Visible = false;

                PingHelper.Start(this._serverIP, 3000, Encoding.Default.GetBytes("Ping Data"),
                    () =>
                    {
                        this.InvokeAction(
                            () =>
                            {
                                label3.Text = "  网络连接状态：" + PingHelper.IPStatus + " 延时：" + PingHelper.RoundtripTime +
                                              "ms" + "  网络异常数：" + PingHelper.ErrorCount + "次";
                            });
                    });
                PingHelper.ErrorAlert(
                    () =>
                    {
                        this.InvokeAction(
                            () =>
                            {
                                label3.Text = "  网络连接状态：" + PingHelper.IPStatus + " 延时：" + PingHelper.RoundtripTime +
                                              "ms" + "  网络异常数：" + PingHelper.ErrorCount + "次" + "  当前网络不稳定，请保持网络通畅！";
                            });
                    });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                label1.Text = "正在等待远程协助";
                textBox1.Enabled =
                    checkBox1.Enabled = comboBox1.Enabled = skinTrackBar1.Enabled = button1.Enabled = true;
                skinProgressBar1.Visible = false;
                PingHelper.Stop();
                label3.Text = "";
            }
        }

        private void remoteServerHelper1_OnOffLined(string msg)
        {
            textBox1.InvokeAction(() =>
            {
                textBox1.Enabled =
                    checkBox1.Enabled = comboBox1.Enabled = skinTrackBar1.Enabled = button1.Enabled = true;
                skinProgressBar1.Visible = false;
                SizedForm(false);
                PingHelper.Stop();
                label3.Text = "";
            });
        }

        /// <summary>
        ///     关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RemoteHelpForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                requestRemoteHelper1.Dispose();
            }
            catch
            {
            }
            try
            {
                remoteScreenUserControl1.Dispose();
            }
            catch
            {
            }
        }

        /// <summary>
        ///     调整大小
        /// </summary>
        /// <param name="max"></param>
        private void SizedForm(bool max)
        {
            if (max)
            {
                FormBorderStyle = FormBorderStyle.Sizable;
                MaximizeBox = true;
                Width = 843;
                Height = 658;
                groupBox1.Visible = true;
            }
            else
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;
                Width = 843;
                Height = 176;
                groupBox1.Visible = false;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _isHelp = checkBox1.Checked;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "高等质量":
                    _qualityLevel = QualityLevelEnum.High;
                    break;
                case "中等质量":
                    _qualityLevel = QualityLevelEnum.Normal;
                    break;
                case "低等质量":
                    _qualityLevel = QualityLevelEnum.Low;
                    break;
                default:
                    _qualityLevel = QualityLevelEnum.Normal;
                    break;
            }
        }

        private void skinTrackBar1_Scroll(object sender, EventArgs e)
        {
            _fps = skinTrackBar1.Value / 2;
            label2.Text = _fps + " fps";
        }
    }
}