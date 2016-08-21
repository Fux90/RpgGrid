using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;
using Utils;
using System.Runtime.Serialization;
using System.Threading;
using RpgGridUserControls.Utilities;
using System.Text;

namespace RpgGridUserControls
{
    [Serializable]
    public partial class CharacterPawn : /*UserControl,*/ GridPawn
    {
        private const string CurrentPfSerializationKey = "currPf";
        private const string MaxPfSerializationKey = "maxPf";
        private const string FacingSerializationKey = "face";
        private const string NotesSerializationKey = "notes";

        private readonly Brush pfBrush = Brushes.Green;
        private readonly Brush underZeroBrush = Brushes.Yellow;
        private readonly Brush damageBrush = Brushes.Red;
        private Pen circlePen = new Pen(Brushes.Black, 2.0f);
        private readonly Pen highlightPen = new Pen(Brushes.YellowGreen, 2.0f);

        private bool dying;

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
                if(currentPf >= 0)
                {
                    dying = false;
                }
                else
                {
                    dying = true;
                }
                this.Invalidate();
                InvalidateTooltipDescription();
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
                InvalidateTooltipDescription();
            }
        }

        private Statistics stats;
        public Statistics Stats
        {
            get
            {
                if (stats == null)
                {
                    stats = new Statistics(this);
                }

                return stats;
            }

            set
            {
                stats = value;
                InvalidateTooltipDescription();
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
                var sem = new Semaphore(0, 1);
                Utils.ApplyCircleMask((Bitmap)value, (res) =>
                {
                    if (res == null)
                    {
                        MessageBox.Show("No");
                    }
                    image = res;
                    sem.Release();
                    this.Invalidate();
                });
                sem.WaitOne();
            }
        }

        public string Notes { get; set; }

        private Rectangle rectImage;
        private Rectangle rectPie;

        private float AnglePf
        {
            get
            {
                if (CurrentPf >= 0)
                {
                    return (float)Math.Max(0, Math.Min(CurrentPf, MaxPf)) * 360.0f / (float)MaxPf;
                }
                else
                {
                    return (float)Math.Max(0, Math.Min(-CurrentPf, 10.0f)) * 360.0f / 10.0f;
                }
            }
        }

        private ContextMenuStrip menuStrip;
        private ContextMenuStrip MenuStrip {
            get
            {
                if(menuStrip == null)
                {
                    menuStrip = CreateContextMenu();
                }

                return menuStrip;
            }
        }

        public CharacterPawn()
        {
            init();
        }

        public CharacterPawn(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            init();

            CurrentPf = info.GetInt32(CurrentPfSerializationKey);
            MaxPf = info.GetInt32(MaxPfSerializationKey);
            Facing = (GridDirections)info.GetValue(FacingSerializationKey, typeof(GridDirections));
            Notes = info.GetString(NotesSerializationKey);

            ComputeRectImage();
        }

        private void init()
        {
            InitializeComponent();

            this.DoubleBuffered = true;
            this.BackColor = Color.Transparent;

            Facing = GridDirections.North;
            pointFace = new Dictionary<GridDirections, Rectangle>();

            this.DoubleClick += OnDoubleClick;
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

        protected void OnDoubleClick(object sender, EventArgs e)
        {
            var mE = (MouseEventArgs)e;

            if(mE.Button == MouseButtons.Left)
            {
                ShowContextMenu(mE.Location);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;

            var anglePf = AnglePf;
            var damagePf = 360.0f - anglePf;

            if (dying)
            {
                g.FillPie(underZeroBrush, rectPie, 0.0f, anglePf);
                g.FillPie(damageBrush, rectPie, anglePf, damagePf);
            }
            else
            {
                g.FillPie(pfBrush, rectPie, 0.0f, anglePf);
                g.FillPie(damageBrush, rectPie, anglePf, damagePf);
            }

            if (Image != null)
            {
                g.DrawImage(Image, rectImage);
            }

            if (MouseIsOver)
            {
                g.DrawEllipse(highlightPen, rectPie);
            }
            else
            {
                g.DrawEllipse(circlePen, rectPie);
            }

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

        private ContextMenuStrip CreateContextMenu()
        {
            var menu = new ContextMenuStrip();

            menu.Items.Add("Assign border color", null, (s,e) =>
            {
                var clrPicker = new ColorPicker();

                if(clrPicker.ShowDialog() == DialogResult.OK)
                {
                    this.circlePen.Color = clrPicker.ChosenColor;
                    this.Invalidate();
                }

            });

            return menu;
        }

        private void ShowContextMenu(Point location)
        {
            ContextMenuStrip = MenuStrip;
            ContextMenuStrip.Show(this, location);
        }

        public override string ToString()
        {
            var strB = new StringBuilder(base.ToString());

            strB.AppendFormat("{0}/{1}", CurrentPf, MaxPf);
            strB.AppendLine(Stats.ToString());

            return strB.ToString();
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
