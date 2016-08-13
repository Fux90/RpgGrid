using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Drawing.Imaging;
using System.Threading;

namespace RpgGridUserControls
{
    [Serializable]
    public partial class CharacterPawnTemplate : UserControl, GridPawnBuilder, ISerializable
    {
        public class Builder : CharacterPawnTemplate
        {
            public static CharacterPawnTemplate Create()
            {
                return new CharacterPawnTemplate()
                {
                    DefaultName = "Npg",
                    DefaultImage = Image.FromFile(@"character2.jpg"),
                    DefaultSize = GridPawn.RpgSize.Medium,
                };
            }
        }

        private const string NameSerializationKey = "name";
        private const string ImageSerializationName = "image";
        private const string ModSizeSerializationName = "size";

        private readonly static Dictionary<GridPawn.RpgSize, Brush> brushBySize;
        private readonly Pen circlePen = new Pen(Brushes.Black, 2.0f);
        private readonly Pen highlightPen = new Pen(Brushes.YellowGreen, 2.0f);

        private string defaultName;
        private Image defaultImage;
        private GridPawn.RpgSize defaultSize;

        public string DefaultName
        {
            get
            {
                return defaultName;
            }

            protected set
            {
                defaultName = value;
            }
        }

        public Image DefaultImage
        {
            get
            {
                return defaultImage;
            }

            protected set
            {
                //defaultImage = Utils.ApplyCircleMask((Bitmap)value);
                var sem = new Semaphore(0,1);
                Utils.ApplyCircleMask((Bitmap)value, (res) =>
                {
                    if(res == null)
                    {
                        MessageBox.Show("No");
                    }
                    defaultImage = res;
                    sem.Release();
                    this.Invalidate();
                });
                sem.WaitOne();
            }
        }

        public new GridPawn.RpgSize DefaultSize
        {
            get
            {
                return defaultSize;
            }

            protected set
            {
                defaultSize = value;
            }
        }

        public bool MouseIsOver { get; private set; }

        private Rectangle rectImage;
        private Rectangle rectPie;

        static CharacterPawnTemplate()
        {
            brushBySize = new Dictionary<GridPawn.RpgSize, Brush>();

            brushBySize[GridPawn.RpgSize.Medium] = Brushes.Green;
            brushBySize[GridPawn.RpgSize.Large] = Brushes.Orange;
            brushBySize[GridPawn.RpgSize.Large_Long] = Brushes.Red;
        }

        protected CharacterPawnTemplate()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        private void CharacterPawnTemplate_Load(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        private void CharacterPawnTemplate_Resize(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        public string generateUniqueName()
        {
            return compact(DateTime.Now);
        }

        private string compact(DateTime now)
        {
            return String.Format(   "{0}{1}{2}{3}{4}{5}", 
                                    now.Year, now.Month, now.Day, 
                                    now.Hour, now.Minute, now.Second);
        }

        private void ComputeRectImage()
        {
            this.Width = this.Height;
            
            int borderX_2, dimX;
            GetDim(Width, out borderX_2, out dimX);
            int borderY_2, dimY;
            GetDim(Height, out borderY_2, out dimY);

            rectImage = new Rectangle(borderX_2, borderY_2, dimX, dimY);
            rectPie = new Rectangle(0, 0, Width, Height);

            var mod = 1;
            var mod2 = 2 * mod;

            var size = new Size(borderX_2 - mod2, borderY_2 - mod2);

            this.Invalidate();
        }

        private void GetDim(int baseDim, out int border_2, out int dim)
        {
            var border = (int)Math.Round(0.30 * baseDim);
            border_2 = border / 2;
            dim = baseDim - 2 * border_2;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            setMouseOver(true);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            setMouseOver(false);
        }

        private void setMouseOver(bool value)
        {
            MouseIsOver = value;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            g.FillEllipse(brushBySize[DefaultSize], rectPie);
            
            if (DefaultImage != null)
            {
                g.DrawImage(DefaultImage, rectImage);
            }

            if (MouseIsOver)
            {
                g.DrawEllipse(highlightPen, rectPie);
            }
            else
            {
                g.DrawEllipse(circlePen, rectPie);
            }
        }

        public GridPawn Build()
        {
            var newPawn = new CharacterPawn();

            newPawn.Name = String.Format("{0}_{1}", DefaultName, generateUniqueName());
            newPawn.Image = DefaultImage;
            newPawn.ModSize = DefaultSize;

            return newPawn;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(NameSerializationKey, DefaultName, typeof(string));
            info.AddValue(ImageSerializationName, DefaultImage, typeof(Image));
            info.AddValue(ModSizeSerializationName, DefaultSize, typeof(GridPawn.RpgSize));
        }
    }
}
