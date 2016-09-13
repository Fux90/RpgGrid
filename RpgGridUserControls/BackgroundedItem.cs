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

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            if (!DesignMode)
            {
                var g = e.Graphics;
                g.DrawImage(BackgroundImage, ClientRectangle);
            }
        }
    }
}
