using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GFF.Component.Capture
{
    public partial class FrmSize : Form
    {
        public FrmSize(Size se) {
            InitializeComponent();
            imageSize = se;
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.TopMost = true;
        }

        private Size imageSize;
        public Size ImageSize {
            get { return imageSize; }
            set { imageSize = value; }
        }

        private void FrmSize_Load(object sender, EventArgs e) {
            textBox1.Text = imageSize.Width.ToString();
            textBox2.Text = imageSize.Height.ToString();
            textBox1.BackColor = Color.White;
            textBox2.BackColor = Color.White;
            button1.Text = "OK";
            button2.Text = "Cancel";
            this.AcceptButton = button1;
            this.CancelButton = button2;
        }

        private void button1_Click(object sender, EventArgs e) {
            if (textBox1.BackColor != Color.White || textBox2.BackColor != Color.White) {
                MessageBox.Show("The input value is invalid!");
                return;
            }
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e) {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void textBox_Validating(object sender, CancelEventArgs e) {
            TextBox tbx = sender as TextBox;
            try {
                int v = int.Parse(tbx.Text);
                if (tbx == textBox1)
                    this.imageSize.Width = v;
                else
                    this.imageSize.Height = v;
                tbx.BackColor = Color.White;
            } catch {
                tbx.BackColor = Color.Yellow;
            }
        }
    }
}
