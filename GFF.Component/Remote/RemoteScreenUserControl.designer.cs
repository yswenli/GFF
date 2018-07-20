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
using System.Windows.Forms;
namespace GFF.Component.Remote
{
    partial class RemoteScreenUserControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // RemoteScreenUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.DoubleBuffered = true;
            this.Name = "RemoteScreenUserControl";
            this.Size = new System.Drawing.Size(800, 450);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.RemoteScreenUserControl_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.RemoteScreenUserControl_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RemoteScreenUserControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RemoteScreenUserControl_MouseMove);
            this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.RemoteScreenUserControl_MouseUp);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.FormRemoteHelpDesktop_MouseWheel);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
