using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RpgGridUserControls
{
    public abstract class BackgroundedItem : UserControl
    {
        private Brush clickBrush = new SolidBrush(Color.FromArgb(100, Color.Yellow));
        private Brush disabledBrush = new SolidBrush(Color.FromArgb(100, Color.Gray));

        private bool clicked;

        protected abstract string ImageResource { get; }

        private Image backgroundImage;
        private Image BackgroundImage
        {
            get
            {
                if (backgroundImage == null)
                {
                    System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly();
                    Stream myStream = myAssembly.GetManifestResourceStream(ImageResource);
                    backgroundImage = new Bitmap(myStream);
                }

                return backgroundImage;
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            clicked = true;
            this.Invalidate();
            base.OnMouseDown(e);
        }

        protected override void OnClick(EventArgs e)
        {
            clicked = false;
            this.Invalidate();
            base.OnClick(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (!DesignMode)
            {
                var g = e.Graphics;
                g.DrawImage(BackgroundImage, ClientRectangle);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;

            if (Enabled == false)
            {
                g.FillRectangle(disabledBrush, this.ClientRectangle);
            }
            else if (clicked)
            {
                g.FillRectangle(clickBrush, this.ClientRectangle);
            }
        }
    }
}
