namespace GFF.Component.Emotion
{
    partial class EmotionForm
    {


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
            this.ImageMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Demo = new System.Windows.Forms.PictureBox();
            this.ImageMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Demo)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageMenu
            // 
            this.ImageMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddItem,
            this.DeleteItem});
            this.ImageMenu.Name = "Menu";
            this.ImageMenu.Size = new System.Drawing.Size(118, 48);
            // 
            // AddItem
            // 
            this.AddItem.Name = "AddItem";
            this.AddItem.Size = new System.Drawing.Size(117, 22);
            this.AddItem.Text = "增加(&A)";
            this.AddItem.Click += new System.EventHandler(this.AddItem_Click);
            // 
            // DeleteItem
            // 
            this.DeleteItem.Name = "DeleteItem";
            this.DeleteItem.Size = new System.Drawing.Size(117, 22);
            this.DeleteItem.Text = "删除(&D)";
            this.DeleteItem.Click += new System.EventHandler(this.DeleteItem_Click);
            // 
            // Demo
            // 
            this.Demo.BackColor = System.Drawing.Color.White;
            this.Demo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Demo.Location = new System.Drawing.Point(5, 5);
            this.Demo.Name = "Demo";
            this.Demo.Size = new System.Drawing.Size(72, 72);
            this.Demo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.Demo.TabIndex = 1;
            this.Demo.TabStop = false;
            // 
            // FrmCountenance
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.CanResize = false;
            this.ClientSize = new System.Drawing.Size(464, 316);
            this.ControlBox = false;
            this.Controls.Add(this.Demo);
            this.DropBack = false;
            this.EffectCaption = CCWin.TitleType.None;
            this.Mobile = CCWin.MobileStyle.None;
            this.Name = "FrmCountenance";
            this.Opacity = 0.9D;
            this.Radius = 4;
            this.Shadow = false;
            this.ShowDrawIcon = false;
            this.ShowInTaskbar = false;
            this.Special = false;
            this.TopMost = true;
            this.ImageMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Demo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion




    }
}