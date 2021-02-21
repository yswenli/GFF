/*****************************************************************************************************
 * 本代码版权归Wenli所有，All Rights Reserved (C) 2016-2016
 *****************************************************************************************************
 * 所属域：WENLI-PC
 * 登录用户：Administrator
 * CLR版本：4.0.30319.18063
 * 唯一标识：cae5b669-bdf6-475b-b954-710a62dba82a
 * 机器名称：WENLI-PC
 * 联系人邮箱：wenguoli_520@qq.com
 *****************************************************************************************************
 * 命名空间：ChatTest
 * 类名称：InitForm
 * 文件名：InitForm
 * 创建年份：2016
 * 创建时间：2016-06-23 20:08:23
 * 创建人：Wenli
 * 创建说明：
 *****************************************************************************************************/

namespace GFFClient
{
    partial class InitForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitForm));
            this.skinWaterTextBox1 = new CCWin.SkinControl.SkinWaterTextBox();
            this.skinButton1 = new CCWin.SkinControl.SkinButton();
            this.skinWaterTextBox2 = new CCWin.SkinControl.SkinWaterTextBox();
            this.skinWaterTextBox3 = new CCWin.SkinControl.SkinWaterTextBox();
            this.SuspendLayout();
            // 
            // skinWaterTextBox1
            // 
            this.skinWaterTextBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinWaterTextBox1.Location = new System.Drawing.Point(47, 78);
            this.skinWaterTextBox1.Name = "skinWaterTextBox1";
            this.skinWaterTextBox1.Size = new System.Drawing.Size(231, 27);
            this.skinWaterTextBox1.TabIndex = 1;
            this.skinWaterTextBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.skinWaterTextBox1.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.skinWaterTextBox1.WaterText = "请输入用户名";
            // 
            // skinButton1
            // 
            this.skinButton1.BackColor = System.Drawing.Color.Transparent;
            this.skinButton1.BaseColor = System.Drawing.Color.LightSkyBlue;
            this.skinButton1.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.skinButton1.DownBack = null;
            this.skinButton1.Location = new System.Drawing.Point(114, 190);
            this.skinButton1.MouseBack = null;
            this.skinButton1.Name = "skinButton1";
            this.skinButton1.NormlBack = null;
            this.skinButton1.Size = new System.Drawing.Size(84, 32);
            this.skinButton1.TabIndex = 0;
            this.skinButton1.Text = "确  定";
            this.skinButton1.UseVisualStyleBackColor = false;
            this.skinButton1.Click += new System.EventHandler(this.skinButton1_Click);
            // 
            // skinWaterTextBox2
            // 
            this.skinWaterTextBox2.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.skinWaterTextBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.skinWaterTextBox2.Location = new System.Drawing.Point(47, 105);
            this.skinWaterTextBox2.Multiline = true;
            this.skinWaterTextBox2.Name = "skinWaterTextBox2";
            this.skinWaterTextBox2.Size = new System.Drawing.Size(231, 62);
            this.skinWaterTextBox2.TabIndex = 2;
            this.skinWaterTextBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.skinWaterTextBox2.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.skinWaterTextBox2.WaterText = "请输入个人简介";
            // 
            // skinWaterTextBox3
            // 
            this.skinWaterTextBox3.ImeMode = System.Windows.Forms.ImeMode.On;
            this.skinWaterTextBox3.Location = new System.Drawing.Point(47, 46);
            this.skinWaterTextBox3.MaxLength = 15;
            this.skinWaterTextBox3.Name = "skinWaterTextBox3";
            this.skinWaterTextBox3.ReadOnly = true;
            this.skinWaterTextBox3.Size = new System.Drawing.Size(231, 25);
            this.skinWaterTextBox3.TabIndex = 3;
            this.skinWaterTextBox3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.skinWaterTextBox3.WaterColor = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(127)))), ((int)(((byte)(127)))));
            this.skinWaterTextBox3.WaterText = "127.0.0.1";
            // 
            // InitForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(328, 229);
            this.Controls.Add(this.skinWaterTextBox3);
            this.Controls.Add(this.skinButton1);
            this.Controls.Add(this.skinWaterTextBox2);
            this.Controls.Add(this.skinWaterTextBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InitForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GFFQ";
            this.Load += new System.EventHandler(this.InitForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CCWin.SkinControl.SkinWaterTextBox skinWaterTextBox1;
        private CCWin.SkinControl.SkinButton skinButton1;
        private CCWin.SkinControl.SkinWaterTextBox skinWaterTextBox2;
        private CCWin.SkinControl.SkinWaterTextBox skinWaterTextBox3;
    }
}