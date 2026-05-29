using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace BalotoAppOnline
{
    public class RoundedButton : Button
    {
        private int borderRadius = 14;
        private Color hoverBackColor = Color.FromArgb(0, 90, 210);
        private Color hoverForeColor = Color.White;
        private Color normalBackColor = Color.FromArgb(0, 69, 166);
        private Color normalForeColor = Color.White;
        private bool isHovered = false;

        public RoundedButton()
        {
            FlatStyle = FlatStyle.Flat;
            FlatAppearance.BorderSize = 0;
            FlatAppearance.MouseOverBackColor = Color.Transparent;
            FlatAppearance.MouseDownBackColor = Color.Transparent;
            BackColor = Color.Transparent;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 11, FontStyle.Regular);
            TextAlign = ContentAlignment.MiddleCenter;
            Cursor = Cursors.Hand;
            SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.SupportsTransparentBackColor, true);
            UpdateStyles();
        }

        public int BorderRadius { get => borderRadius; set { borderRadius = value; Invalidate(); } }
        public Color HoverBackColor { get => hoverBackColor; set { hoverBackColor = value; Invalidate(); } }
        public Color HoverForeColor { get => hoverForeColor; set { hoverForeColor = value; Invalidate(); } }
        public Color NormalBackColor { get => normalBackColor; set { normalBackColor = value; Invalidate(); } }
        public Color NormalForeColor { get => normalForeColor; set { normalForeColor = value; Invalidate(); } }

        protected override void OnMouseEnter(EventArgs e) { base.OnMouseEnter(e); isHovered = true; Invalidate(); }
        protected override void OnMouseLeave(EventArgs e) { base.OnMouseLeave(e); isHovered = false; Invalidate(); }

        private Color ObtenerColorFondoRaiz()
        {
            Control c = this.Parent;
            while (c != null)
            {
                if (c.BackColor != Color.Transparent && c is Form)
                    return c.BackColor;
                c = c.Parent;
            }
            return Color.FromArgb(240, 242, 245);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Color fondoRaiz = ObtenerColorFondoRaiz();
            Color fillColor = isHovered ? hoverBackColor : normalBackColor;
            Color textColor = isHovered ? hoverForeColor : normalForeColor;

            using (SolidBrush bgBrush = new SolidBrush(fondoRaiz))
                g.FillRectangle(bgBrush, 0, 0, Width, Height);

            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = CrearPath(rect, borderRadius))
            using (SolidBrush fill = new SolidBrush(fillColor))
                g.FillPath(fill, path);

            StringFormat sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
                Trimming = StringTrimming.EllipsisCharacter
            };
            using (SolidBrush tb = new SolidBrush(textColor))
                g.DrawString(Text, Font, tb, new RectangleF(0, 0, Width, Height), sf);
        }

        private GraphicsPath CrearPath(Rectangle r, int radius)
        {
            int rad = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
            GraphicsPath p = new GraphicsPath();
            p.AddArc(r.X, r.Y, rad, rad, 180, 90);
            p.AddArc(r.Right - rad, r.Y, rad, rad, 270, 90);
            p.AddArc(r.Right - rad, r.Bottom - rad, rad, rad, 0, 90);
            p.AddArc(r.X, r.Bottom - rad, rad, rad, 90, 90);
            p.CloseFigure();
            return p;
        }
    }
}