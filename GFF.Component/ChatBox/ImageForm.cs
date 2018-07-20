using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GFF.Component.ChatBox
{
    public partial class ImageForm : Form
    {
        public ImageForm(Image img)
        {
            InitializeComponent();
            if (img == null)
            {
                return;
            }

            int maxLen = img.Width > img.Height ? img.Width : img.Height;
            maxLen += 40;
            if (maxLen > Screen.PrimaryScreen.Bounds.Width || maxLen > Screen.PrimaryScreen.Bounds.Height)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                this.Size = new Size(maxLen, maxLen);
            }

            this.gifBox1.Image = img;            
            this.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            this.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            this.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            this.KeyUp += new KeyEventHandler(ImageForm_KeyUp);

            this.gifBox1.MouseDown += new MouseEventHandler(pictureBox1_MouseDown);
            this.gifBox1.MouseUp += new MouseEventHandler(pictureBox1_MouseUp);
            this.gifBox1.MouseMove += new MouseEventHandler(pictureBox1_MouseMove);
            this.gifBox1.KeyUp += new KeyEventHandler(ImageForm_KeyUp);
        }

        void ImageForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
        

        void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.moving)
            {
                int delt = Math.Abs(e.Location.X - this.originMouseLocation.X) + Math.Abs(e.Location.Y - this.originMouseLocation.Y);               
                if (delt >= 4)
                {
                    this.Location = new Point(this.Location.X + e.Location.X - this.originMouseLocation.X, this.Location.Y + e.Location.Y - this.originMouseLocation.Y);  
                }
            }
        }

        void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.moving = false;
                this.Cursor = Cursors.Default;
            }
        }

        private bool moving = false;
        private Point originMouseLocation;       
        void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (this.WindowState == FormWindowState.Maximized)
                {
                    return;
                }

                this.moving = true;
                this.originMouseLocation = e.Location;              
                this.Cursor = Cursors.Hand;
            }
        }      

        private void panel1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
