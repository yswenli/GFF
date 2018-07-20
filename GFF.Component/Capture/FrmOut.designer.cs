namespace GFF.Component.Capture
{
    partial class FrmOut
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmOut));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.TMenuItem_OriginalToClip = new System.Windows.Forms.ToolStripMenuItem();
            this.TMenuItem_CurrentToClip = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOringinalToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.TMenuItem_SaveOriginal = new System.Windows.Forms.ToolStripMenuItem();
            this.TMenuItem_SaveCurrent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.TMenuItem_Size = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.TMenuItem_Help = new System.Windows.Forms.ToolStripMenuItem();
            this.TMenuItem_Close = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TMenuItem_OriginalToClip,
            this.TMenuItem_CurrentToClip,
            this.saveOringinalToolStripMenuItem,
            this.TMenuItem_SaveOriginal,
            this.TMenuItem_SaveCurrent,
            this.toolStripSeparator1,
            this.TMenuItem_Size,
            this.toolStripSeparator2,
            this.TMenuItem_Help,
            this.TMenuItem_Close});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(201, 176);
            // 
            // TMenuItem_OriginalToClip
            // 
            this.TMenuItem_OriginalToClip.Name = "TMenuItem_OriginalToClip";
            this.TMenuItem_OriginalToClip.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_OriginalToClip.Text = "Original to ClipBoard";
            this.TMenuItem_OriginalToClip.Click += new System.EventHandler(this.TMenuItem_OriginalToClip_Click);
            // 
            // TMenuItem_CurrentToClip
            // 
            this.TMenuItem_CurrentToClip.Name = "TMenuItem_CurrentToClip";
            this.TMenuItem_CurrentToClip.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_CurrentToClip.Text = "Current to ClipBoard";
            this.TMenuItem_CurrentToClip.Click += new System.EventHandler(this.TMenuItem_CurrentToClip_Click);
            // 
            // saveOringinalToolStripMenuItem
            // 
            this.saveOringinalToolStripMenuItem.Name = "saveOringinalToolStripMenuItem";
            this.saveOringinalToolStripMenuItem.Size = new System.Drawing.Size(197, 6);
            // 
            // TMenuItem_SaveOriginal
            // 
            this.TMenuItem_SaveOriginal.Name = "TMenuItem_SaveOriginal";
            this.TMenuItem_SaveOriginal.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_SaveOriginal.Text = "Save Original";
            this.TMenuItem_SaveOriginal.Click += new System.EventHandler(this.TMenuItem_SaveOriginal_Click);
            // 
            // TMenuItem_SaveCurrent
            // 
            this.TMenuItem_SaveCurrent.Name = "TMenuItem_SaveCurrent";
            this.TMenuItem_SaveCurrent.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_SaveCurrent.Text = "Save Current";
            this.TMenuItem_SaveCurrent.Click += new System.EventHandler(this.TMenuItem_SaveCurrent_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // TMenuItem_Size
            // 
            this.TMenuItem_Size.Name = "TMenuItem_Size";
            this.TMenuItem_Size.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_Size.Text = "Set Size";
            this.TMenuItem_Size.Click += new System.EventHandler(this.TMenuItem_Size_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(197, 6);
            // 
            // TMenuItem_Help
            // 
            this.TMenuItem_Help.Name = "TMenuItem_Help";
            this.TMenuItem_Help.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_Help.Text = "Help";
            this.TMenuItem_Help.Click += new System.EventHandler(this.TMenuItem_Help_Click);
            // 
            // TMenuItem_Close
            // 
            this.TMenuItem_Close.Name = "TMenuItem_Close";
            this.TMenuItem_Close.Size = new System.Drawing.Size(200, 22);
            this.TMenuItem_Close.Text = "Close";
            this.TMenuItem_Close.Click += new System.EventHandler(this.TMenuItem_Close_Click);
            // 
            // FrmOut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(290, 247);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmOut";
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_OriginalToClip;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_CurrentToClip;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_SaveOriginal;
        private System.Windows.Forms.ToolStripSeparator saveOringinalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_SaveCurrent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_Help;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_Close;
        private System.Windows.Forms.ToolStripMenuItem TMenuItem_Size;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}