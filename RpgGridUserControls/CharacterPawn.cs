using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Utils;
using System.Runtime.Serialization;

namespace RpgGridUserControls
{
    public partial class CharacterPawn : /*UserControl,*/ GridPawn
    {
        private const string CurrentPfSerializationKey = "currPf";
        private const string MaxPfSerializationKey = "maxPf";
        private const string FacingSerializationKey = "face";
        private const string NotesSerializationKey = "notes";

        private readonly Brush pfBrush = Brushes.Green;
        private readonly Brush damageBrush = Brushes.Red;
        private readonly Pen circlePen = new Pen(Brushes.Black, 2.0f);

        private int currentPf;
        public int CurrentPf
        {
            get
            {
                return currentPf;
            }

            set
            {
                currentPf = Math.Max(-10, value);
                this.Invalidate();
            }
        }

        private int maxPf;
        public int MaxPf
        {
            get
            {
                return Math.Max(1, maxPf);
            }

            set
            {
                maxPf = value;
                this.Invalidate();
            }
        }

        public GridDirections Facing { get; private set; }

        private int numGridDirections = -1;
        protected int NumGridDirections
        {
            get
            {
                if (numGridDirections == -1)
                {
                    numGridDirections = Enum.GetValues(typeof(GridDirections)).Length;
                }
                return numGridDirections;
            }
        }

        private Dictionary<GridDirections, Rectangle> pointFace;

        private Image image;
        public override Image Image
        {
            get
            {
                return image;
            }

            set
            {
                image = ApplyCircleMask((Bitmap)value);
                this.Invalidate();
            }
        }

        public string Notes { get; set; }

        private Rectangle rectImage;
        private Rectangle rectPie;

        private float AnglePf
        {
            get
            {
                return (float)Math.Max(0, Math.Min(CurrentPf, MaxPf)) * 360.0f / (float)MaxPf;
            }
        }

        private Image ApplyCircleMask(Bitmap inputImage)
        {
            if (inputImage != null)
            {
                var img = new Bitmap(inputImage.Width, inputImage.Height, PixelFormat.Format32bppArgb);
                var alphaChannel = new Bitmap(inputImage.Width, inputImage.Height);
                var g = Graphics.FromImage(alphaChannel);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                var rect = new Rectangle(new Point(), new Size(alphaChannel.Width, alphaChannel.Height));

                g.FillRectangle(Brushes.White, rect);
                g.FillEllipse(Brushes.Black, rect);

                for (int r = 0; r < img.Height; r++)
                {
                    for (int c = 0; c < img.Width; c++)
                    {
                        if (alphaChannel.GetPixel(c, r).R == (byte)0)
                        {
                            img.SetPixel(c, r, Color.FromArgb(255, inputImage.GetPixel(c, r)));
                        }
                    }
                }

                var gOut = Graphics.FromImage(img);
                gOut.DrawEllipse(circlePen, rect);

                return img;
            }
            else
            {
                return null;
            }
        }

        public CharacterPawn()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;

            Facing = GridDirections.North;
            pointFace = new Dictionary<GridDirections, Rectangle>();
        }

        public CharacterPawn(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }

        public override void PerformRotate90Degrees()
        {
            Facing = (GridDirections)(((int)Facing + 1) % NumGridDirections);
            base.PerformRotate90Degrees();
        }

        private void CharacterPawn_Load(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        private void CharacterPawn_Resize(object sender, EventArgs e)
        {
            ComputeRectImage();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var anglePf = AnglePf;
            var damagePf = 360.0f - anglePf;

            g.FillPie(pfBrush, rectPie, 0.0f, anglePf);
            g.FillPie(damageBrush, rectPie, anglePf, damagePf);

            if (Image != null)
            {
                g.DrawImage(Image, rectImage);
            }

            g.DrawEllipse(circlePen, rectPie);
            g.FillEllipse(Brushes.Blue, pointFace[Facing]);
        }

        private void ComputeRectImage()
        {
            //if (IsSquared)
            //{
            //    //this.Width = this.Height;
            //}

            int borderX_2, dimX;
            GetDim(Width, out borderX_2, out dimX);
            int borderY_2, dimY;
            GetDim(Height, out borderY_2, out dimY);

            rectImage = new Rectangle(borderX_2, borderY_2, dimX, dimY);
            rectPie = new Rectangle(0, 0, Width, Height);

            var mod = 1;
            var mod2 = 2 * mod;

            int x = GetFacePoint(Width, borderX_2, mod);
            int y = GetFacePoint(Height, borderY_2, mod);

            var size = new Size(borderX_2 - mod2, borderY_2 - mod2);

            pointFace[GridDirections.North] = new Rectangle(new Point(x, mod), size);
            pointFace[GridDirections.East] = new Rectangle(new Point(Width - borderX_2 + mod, y), size);
            pointFace[GridDirections.South] = new Rectangle(new Point(x, Height - borderY_2 + mod), size);
            pointFace[GridDirections.West] = new Rectangle(new Point(mod, y), size);

            this.Invalidate();
        }

        private int GetFacePoint(int baseDim, int border_2, int mod)
        {
            var border_4 = border_2 / 2;
            var dim_2 = baseDim / 2;
            var coord = dim_2 - border_4 + mod;
            return coord;
        }

        private void GetDim(int baseDim, out int border_2, out int dim)
        {
            var border = (int)Math.Round(0.30 * baseDim);
            border_2 = border / 2;
            dim = baseDim - 2 * border_2;
        }

        public override Point PositionAtNoZoomNoMargin
        {
            get
            {
                return new Point()
                {
                    X = PositionAtNoZoom.X + rectImage.X,
                    Y = PositionAtNoZoom.Y + rectImage.Y,
                };
            }
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(CurrentPfSerializationKey, CurrentPf, typeof(int));
            info.AddValue(MaxPfSerializationKey, MaxPf, typeof(int));
            info.AddValue(FacingSerializationKey, Facing, typeof(GridDirections));
            info.AddValue(NotesSerializationKey, Notes, typeof(string));
        }
    }
}
