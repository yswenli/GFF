using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using CCWin.SkinControl;

namespace GFF.Component.Capture
{
    [Designer(typeof(ColorBoxDesginer))]
    public partial class ColorBox : Control
    {
        public ColorBox() {
            InitializeComponent();
            selectedColor = Color.Red;
            m_rectSelected = new Rectangle(-100, -100, 14, 14);

            this.SetStyle(ControlStyles.ResizeRedraw, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        private Color selectedColor;
        public Color SelectedColor {
            get { return selectedColor; }
        }

        private Point m_ptCurrent;
        private Rectangle m_rectSelected;
        private Bitmap m_clrImage = global::GFF.Component.Properties.Resources.color;
        private Color m_lastColor;

        public delegate void ColorChangedHandler(object sender, ColorChangedEventArgs e);
        public event ColorChangedHandler ColorChanged;
        protected virtual void OnColorChanged(ColorChangedEventArgs e) {
            if (this.ColorChanged != null)
                ColorChanged(this, e);
        }

        protected override void OnClick(EventArgs e) {
            Color clr = m_clrImage.GetPixel(m_ptCurrent.X, m_ptCurrent.Y);
            if (clr.ToArgb() != Color.FromArgb(255, 254, 254, 254).ToArgb()
                && clr.ToArgb() != Color.FromArgb(255, 133, 141, 151).ToArgb()
                && clr.ToArgb() != Color.FromArgb(255, 110, 126, 149).ToArgb()) {
                if (this.selectedColor != clr)
                    this.selectedColor = clr;
                this.Invalidate();
                this.OnColorChanged(new ColorChangedEventArgs(clr));
            }
            base.OnClick(e);
        }

        protected override void OnMouseMove(MouseEventArgs e) {
            m_ptCurrent = e.Location;
            try {
                Color clr = m_clrImage.GetPixel(m_ptCurrent.X, m_ptCurrent.Y);
                if (clr != m_lastColor) {
                    if (clr.ToArgb() != Color.FromArgb(255, 254, 254, 254).ToArgb()
                        && clr.ToArgb() != Color.FromArgb(255, 133, 141, 151).ToArgb()
                        && clr.ToArgb() != Color.FromArgb(255, 110, 126, 149).ToArgb()
                        && e.X > 39) {
                        m_rectSelected.Y = e.Y > 17 ? 17 : 2;
                        m_rectSelected.X = ((e.X - 39) / 15) * 15 + 38;
                        this.Invalidate();
                    } else {
                        m_rectSelected.X = m_rectSelected.Y = -100;
                        this.Invalidate();
                    }
                }
                m_lastColor = clr;
            } finally {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseLeave(EventArgs e) {
            m_rectSelected.X = m_rectSelected.Y = -100;
            this.Invalidate();
            base.OnMouseLeave(e);
        }

        protected override void OnPaint(PaintEventArgs e) {
            Graphics g = e.Graphics;
            g.DrawImage(global::GFF.Component.Properties.Resources.color,
                new Rectangle(0, 0, 165, 35));
            g.DrawRectangle(Pens.SteelBlue, 0, 0, 164, 34);
            SolidBrush sb = new SolidBrush(selectedColor);
            g.FillRectangle(sb, 9, 5, 24, 24);
            g.DrawRectangle(Pens.DarkCyan, m_rectSelected);
            base.OnPaint(e);
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            base.SetBoundsCore(x, y, 165, 35, specified);
        }
    }

    public class ColorChangedEventArgs : EventArgs
    {
        private Color color;
        public Color Color {
            get { return color; }
        }

        public ColorChangedEventArgs(Color clr) {
            this.color = clr;
        }
    }
}
