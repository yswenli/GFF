namespace GFF.Component.Capture
{
    partial class CaptureForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaptureForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.toolButton1 = new GFF.Component.Capture.ToolButton();
            this.toolButton3 = new GFF.Component.Capture.ToolButton();
            this.toolButton2 = new GFF.Component.Capture.ToolButton();
            this.colorBox1 = new GFF.Component.Capture.ColorBox();
            this.tBtn_Out = new GFF.Component.Capture.ToolButton();
            this.tBtn_Finish = new GFF.Component.Capture.ToolButton();
            this.tBtn_Close = new GFF.Component.Capture.ToolButton();
            this.tBtn_Save = new GFF.Component.Capture.ToolButton();
            this.tBtn_Cancel = new GFF.Component.Capture.ToolButton();
            this.tBtn_Text = new GFF.Component.Capture.ToolButton();
            this.tBtn_Brush = new GFF.Component.Capture.ToolButton();
            this.tBtn_Arrow = new GFF.Component.Capture.ToolButton();
            this.tBtn_Ellipse = new GFF.Component.Capture.ToolButton();
            this.tBtn_Rect = new GFF.Component.Capture.ToolButton();
            this.imageProcessBox1 = new GFF.Component.Capture.ImageProcessBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tBtn_Out);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.tBtn_Finish);
            this.panel1.Controls.Add(this.tBtn_Close);
            this.panel1.Controls.Add(this.tBtn_Save);
            this.panel1.Controls.Add(this.tBtn_Cancel);
            this.panel1.Controls.Add(this.tBtn_Text);
            this.panel1.Controls.Add(this.tBtn_Brush);
            this.panel1.Controls.Add(this.tBtn_Arrow);
            this.panel1.Controls.Add(this.tBtn_Ellipse);
            this.panel1.Controls.Add(this.tBtn_Rect);
            this.panel1.Location = new System.Drawing.Point(12, 83);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(321, 25);
            this.panel1.TabIndex = 1;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::GFF.Component.Properties.Resources.separator;
            this.pictureBox2.Location = new System.Drawing.Point(226, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(1, 17);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox2.TabIndex = 10;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::GFF.Component.Properties.Resources.separator;
            this.pictureBox1.Location = new System.Drawing.Point(140, 6);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(1, 17);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 9;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.toolButton1);
            this.panel2.Controls.Add(this.toolButton3);
            this.panel2.Controls.Add(this.toolButton2);
            this.panel2.Controls.Add(this.colorBox1);
            this.panel2.Location = new System.Drawing.Point(12, 114);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 32);
            this.panel2.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Location = new System.Drawing.Point(12, 24);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 19);
            this.textBox1.TabIndex = 3;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            this.textBox1.Resize += new System.EventHandler(this.textBox1_Resize);
            this.textBox1.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // toolButton1
            // 
            this.toolButton1.BtnImage = global::GFF.Component.Properties.Resources.small;
            this.toolButton1.IsSelected = true;
            this.toolButton1.IsSelectedBtn = true;
            this.toolButton1.IsSingleSelectedBtn = true;
            this.toolButton1.Location = new System.Drawing.Point(3, 6);
            this.toolButton1.Name = "toolButton1";
            this.toolButton1.Size = new System.Drawing.Size(21, 21);
            this.toolButton1.TabIndex = 4;
            // 
            // toolButton3
            // 
            this.toolButton3.BtnImage = global::GFF.Component.Properties.Resources.large;
            this.toolButton3.IsSelected = false;
            this.toolButton3.IsSelectedBtn = true;
            this.toolButton3.IsSingleSelectedBtn = true;
            this.toolButton3.Location = new System.Drawing.Point(57, 6);
            this.toolButton3.Name = "toolButton3";
            this.toolButton3.Size = new System.Drawing.Size(21, 21);
            this.toolButton3.TabIndex = 3;
            // 
            // toolButton2
            // 
            this.toolButton2.BtnImage = global::GFF.Component.Properties.Resources.middle;
            this.toolButton2.IsSelected = false;
            this.toolButton2.IsSelectedBtn = true;
            this.toolButton2.IsSingleSelectedBtn = true;
            this.toolButton2.Location = new System.Drawing.Point(30, 6);
            this.toolButton2.Name = "toolButton2";
            this.toolButton2.Size = new System.Drawing.Size(21, 21);
            this.toolButton2.TabIndex = 2;
            // 
            // colorBox1
            // 
            this.colorBox1.Location = new System.Drawing.Point(85, 0);
            this.colorBox1.Name = "colorBox1";
            this.colorBox1.Size = new System.Drawing.Size(165, 35);
            this.colorBox1.TabIndex = 0;
            this.colorBox1.Text = "colorBox1";
            // 
            // tBtn_Out
            // 
            this.tBtn_Out.BtnImage = global::GFF.Component.Properties.Resources._out;
            this.tBtn_Out.IsSelected = false;
            this.tBtn_Out.IsSelectedBtn = false;
            this.tBtn_Out.IsSingleSelectedBtn = false;
            this.tBtn_Out.Location = new System.Drawing.Point(174, 3);
            this.tBtn_Out.Name = "tBtn_Out";
            this.tBtn_Out.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Out.TabIndex = 11;
            this.tBtn_Out.Click += new System.EventHandler(this.tBtn_Out_Click);
            // 
            // tBtn_Finish
            // 
            this.tBtn_Finish.BtnImage = global::GFF.Component.Properties.Resources.ok;
            this.tBtn_Finish.IsSelected = false;
            this.tBtn_Finish.IsSelectedBtn = false;
            this.tBtn_Finish.IsSingleSelectedBtn = false;
            this.tBtn_Finish.Location = new System.Drawing.Point(262, 3);
            this.tBtn_Finish.Name = "tBtn_Finish";
            this.tBtn_Finish.Size = new System.Drawing.Size(50, 21);
            this.tBtn_Finish.TabIndex = 8;
            this.tBtn_Finish.Text = "完成";
            this.tBtn_Finish.Click += new System.EventHandler(this.tBtn_Finish_Click);
            // 
            // tBtn_Close
            // 
            this.tBtn_Close.BtnImage = global::GFF.Component.Properties.Resources.close;
            this.tBtn_Close.IsSelected = false;
            this.tBtn_Close.IsSelectedBtn = false;
            this.tBtn_Close.IsSingleSelectedBtn = false;
            this.tBtn_Close.Location = new System.Drawing.Point(235, 3);
            this.tBtn_Close.Name = "tBtn_Close";
            this.tBtn_Close.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Close.TabIndex = 7;
            // 
            // tBtn_Save
            // 
            this.tBtn_Save.BtnImage = global::GFF.Component.Properties.Resources.save;
            this.tBtn_Save.IsSelected = false;
            this.tBtn_Save.IsSelectedBtn = false;
            this.tBtn_Save.IsSingleSelectedBtn = false;
            this.tBtn_Save.Location = new System.Drawing.Point(201, 3);
            this.tBtn_Save.Name = "tBtn_Save";
            this.tBtn_Save.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Save.TabIndex = 6;
            this.tBtn_Save.Click += new System.EventHandler(this.tBtn_Save_Click);
            // 
            // tBtn_Cancel
            // 
            this.tBtn_Cancel.BtnImage = global::GFF.Component.Properties.Resources.cancel;
            this.tBtn_Cancel.IsSelected = false;
            this.tBtn_Cancel.IsSelectedBtn = false;
            this.tBtn_Cancel.IsSingleSelectedBtn = false;
            this.tBtn_Cancel.Location = new System.Drawing.Point(147, 3);
            this.tBtn_Cancel.Name = "tBtn_Cancel";
            this.tBtn_Cancel.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Cancel.TabIndex = 5;
            this.tBtn_Cancel.Click += new System.EventHandler(this.tBtn_Cancel_Click);
            // 
            // tBtn_Text
            // 
            this.tBtn_Text.BtnImage = global::GFF.Component.Properties.Resources.text;
            this.tBtn_Text.IsSelected = false;
            this.tBtn_Text.IsSelectedBtn = true;
            this.tBtn_Text.IsSingleSelectedBtn = false;
            this.tBtn_Text.Location = new System.Drawing.Point(113, 3);
            this.tBtn_Text.Name = "tBtn_Text";
            this.tBtn_Text.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Text.TabIndex = 4;
            // 
            // tBtn_Brush
            // 
            this.tBtn_Brush.BtnImage = global::GFF.Component.Properties.Resources.brush;
            this.tBtn_Brush.IsSelected = false;
            this.tBtn_Brush.IsSelectedBtn = true;
            this.tBtn_Brush.IsSingleSelectedBtn = false;
            this.tBtn_Brush.Location = new System.Drawing.Point(86, 3);
            this.tBtn_Brush.Name = "tBtn_Brush";
            this.tBtn_Brush.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Brush.TabIndex = 3;
            // 
            // tBtn_Arrow
            // 
            this.tBtn_Arrow.BtnImage = global::GFF.Component.Properties.Resources.arrow;
            this.tBtn_Arrow.IsSelected = false;
            this.tBtn_Arrow.IsSelectedBtn = true;
            this.tBtn_Arrow.IsSingleSelectedBtn = false;
            this.tBtn_Arrow.Location = new System.Drawing.Point(59, 3);
            this.tBtn_Arrow.Name = "tBtn_Arrow";
            this.tBtn_Arrow.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Arrow.TabIndex = 2;
            // 
            // tBtn_Ellipse
            // 
            this.tBtn_Ellipse.BtnImage = global::GFF.Component.Properties.Resources.ellips;
            this.tBtn_Ellipse.IsSelected = false;
            this.tBtn_Ellipse.IsSelectedBtn = true;
            this.tBtn_Ellipse.IsSingleSelectedBtn = false;
            this.tBtn_Ellipse.Location = new System.Drawing.Point(32, 3);
            this.tBtn_Ellipse.Name = "tBtn_Ellipse";
            this.tBtn_Ellipse.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Ellipse.TabIndex = 1;
            // 
            // tBtn_Rect
            // 
            this.tBtn_Rect.BtnImage = global::GFF.Component.Properties.Resources.rect;
            this.tBtn_Rect.IsSelected = false;
            this.tBtn_Rect.IsSelectedBtn = true;
            this.tBtn_Rect.IsSingleSelectedBtn = false;
            this.tBtn_Rect.Location = new System.Drawing.Point(6, 3);
            this.tBtn_Rect.Name = "tBtn_Rect";
            this.tBtn_Rect.Size = new System.Drawing.Size(21, 21);
            this.tBtn_Rect.TabIndex = 0;
            // 
            // imageProcessBox1
            // 
            this.imageProcessBox1.BackColor = System.Drawing.Color.Black;
            this.imageProcessBox1.BaseImage = null;
            this.imageProcessBox1.CanReset = true;
            this.imageProcessBox1.Cursor = System.Windows.Forms.Cursors.Default;
            this.imageProcessBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.imageProcessBox1.ForeColor = System.Drawing.Color.White;
            this.imageProcessBox1.Location = new System.Drawing.Point(0, 0);
            this.imageProcessBox1.Name = "imageProcessBox1";
            this.imageProcessBox1.Size = new System.Drawing.Size(363, 247);
            this.imageProcessBox1.TabIndex = 0;
            this.imageProcessBox1.Text = "imageProcessBox1";
            this.imageProcessBox1.Paint += new System.Windows.Forms.PaintEventHandler(this.imageProcessBox1_Paint);
            this.imageProcessBox1.DoubleClick += new System.EventHandler(this.imageProcessBox1_DoubleClick);
            this.imageProcessBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.imageProcessBox1_MouseDown);
            this.imageProcessBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.imageProcessBox1_MouseMove);
            this.imageProcessBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.imageProcessBox1_MouseUp);
            // 
            // FrmCapture
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 247);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.imageProcessBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmCapture";
            this.Text = "FrmCapture";
            this.Load += new System.EventHandler(this.FrmCapture_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private ToolButton tBtn_Ellipse;
        private ToolButton tBtn_Rect;
        private ToolButton tBtn_Arrow;
        private ToolButton tBtn_Brush;
        private ToolButton tBtn_Text;
        private ToolButton tBtn_Finish;
        private ToolButton tBtn_Close;
        private ToolButton tBtn_Save;
        private ToolButton tBtn_Cancel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private ImageProcessBox imageProcessBox1;
        private System.Windows.Forms.Panel panel2;
        private ColorBox colorBox1;
        private ToolButton toolButton1;
        private ToolButton toolButton3;
        private ToolButton toolButton2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Timer timer1;
        private ToolButton tBtn_Out;

    }
}